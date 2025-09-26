using Microsoft.Extensions.Logging;
using ProductManagement.dtos;
using ProductManagement.entities;
using ProductManagement.helpers;
using ProductManagement.infrastructure.repositories;
using ProductManagement.models;
using Bogus;

namespace ProductManagement.services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<ProductResponseDto>> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
                return Result<ProductResponseDto>.Failure("Invalid product ID");

            var product = await _productRepository.GetProductDtoByIdAsync(id);
            return product != null
                ? Result<ProductResponseDto>.Success(product)
                : Result<ProductResponseDto>.Failure("Product not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            return Result<ProductResponseDto>.Failure("An error occurred while retrieving the product");
        }
    }

    public async Task<Result<PagedResultDto<ProductSummaryDto>>> GetPagedAsync(PaginationRequestDto request)
    {
        try
        {
            var result = await _productRepository.GetProductDtosAsync(request);
            return Result<PagedResultDto<ProductSummaryDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paginated products");
            return Result<PagedResultDto<ProductSummaryDto>>.Failure("An error occurred while retrieving products");
        }
    }

    public async Task<Result<ProductResponseDto>> CreateAsync(CreateProductRequestDto request)
    {
        try
        {
            // Check if SKU already exists
            if (await _productRepository.ExistsBySkuAsync(request.Sku))
            {
                return Result<ProductResponseDto>.Failure("A product with this SKU already exists");
            }

            var product = request.ToEntity();

            // Add color relationships
            if (request.ColorIds.Any())
            {
                var availableColors = await _productRepository.GetColorsAsync();
                var validColorIds = availableColors.Select(c => c.Id).ToHashSet();
                
                foreach (var colorId in request.ColorIds.Where(id => validColorIds.Contains(id)))
                {
                    product.ProductColors.Add(new ProductColor
                    {
                        Product = product,
                        ColorId = colorId
                    });
                }
            }

            // Add size relationships
            if (request.SizeIds.Any())
            {
                var availableSizes = await _productRepository.GetSizesAsync();
                var validSizeIds = availableSizes.Select(s => s.Id).ToHashSet();
                
                foreach (var sizeId in request.SizeIds.Where(id => validSizeIds.Contains(id)))
                {
                    product.ProductSizes.Add(new ProductSize
                    {
                        Product = product,
                        SizeId = sizeId
                    });
                }
            }

            var createdProduct = await _productRepository.AddAsync(product);
            var responseDto = await _productRepository.GetProductDtoByIdAsync(createdProduct.Id);
            
            return Result<ProductResponseDto>.Success(responseDto!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return Result<ProductResponseDto>.Failure("An error occurred while creating the product");
        }
    }

    public async Task<Result<ProductResponseDto>> UpdateAsync(int id, UpdateProductRequestDto request)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return Result<ProductResponseDto>.Failure("Product not found");

            // Check if SKU already exists (excluding current product)
            if (await _productRepository.ExistsBySkuAsync(request.Sku, id))
            {
                return Result<ProductResponseDto>.Failure("A product with this SKU already exists");
            }

            product.UpdateFromDto(request);

            // Update color relationships
            product.ProductColors.Clear();
            if (request.ColorIds.Any())
            {
                var availableColors = await _productRepository.GetColorsAsync();
                var validColorIds = availableColors.Select(c => c.Id).ToHashSet();
                
                foreach (var colorId in request.ColorIds.Where(id => validColorIds.Contains(id)))
                {
                    product.ProductColors.Add(new ProductColor
                    {
                        Product = product,
                        ColorId = colorId
                    });
                }
            }

            // Update size relationships
            product.ProductSizes.Clear();
            if (request.SizeIds.Any())
            {
                var availableSizes = await _productRepository.GetSizesAsync();
                var validSizeIds = availableSizes.Select(s => s.Id).ToHashSet();
                
                foreach (var sizeId in request.SizeIds.Where(id => validSizeIds.Contains(id)))
                {
                    product.ProductSizes.Add(new ProductSize
                    {
                        Product = product,
                        SizeId = sizeId
                    });
                }
            }

            await _productRepository.UpdateAsync(product);
            var responseDto = await _productRepository.GetProductDtoByIdAsync(id);
            
            return Result<ProductResponseDto>.Success(responseDto!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
            return Result<ProductResponseDto>.Failure("An error occurred while updating the product");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return Result.Failure("Product not found");

            await _productRepository.DeleteAsync(product);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
            return Result.Failure("An error occurred while deleting the product");
        }
    }

    public async Task<Result<List<ProductResponseDto>>> SeedAsync(int count)
    {
        try
        {
            if (count < 1 || count > 10000)
                return Result<List<ProductResponseDto>>.Failure("Count must be between 1 and 10,000");

            var colors = await _productRepository.GetColorsAsync();
            var sizes = await _productRepository.GetSizesAsync();

            if (!colors.Any() || !sizes.Any())
                return Result<List<ProductResponseDto>>.Failure("Colors or sizes not available. Please ensure database is properly seeded with reference data.");

            var faker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Model, f => f.Vehicle.Model())
                .RuleFor(p => p.Brand, f => f.Company.CompanyName())
                .RuleFor(p => p.Sku, f => f.Commerce.Ean13())
                .RuleFor(p => p.Price, f => f.Random.Decimal(10, 1000))
                .RuleFor(p => p.Category, f => f.Commerce.Categories(1).First())
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(1))
                .RuleFor(p => p.UpdatedAt, (f, p) => p.CreatedAt.AddDays(f.Random.Int(0, 30)));

            var products = faker.Generate(count);
            var results = new List<ProductResponseDto>();

            foreach (var product in products)
            {
                // Ensure unique SKU
                while (await _productRepository.ExistsBySkuAsync(product.Sku))
                {
                    product.Sku = new Faker().Commerce.Ean13();
                }

                // Add random colors (1-3)
                var randomColors = colors.OrderBy(x => Guid.NewGuid()).Take(new Random().Next(1, 4));
                foreach (var color in randomColors)
                {
                    product.ProductColors.Add(new ProductColor
                    {
                        Product = product,
                        ColorId = color.Id
                    });
                }

                // Add random sizes (1-3)
                var randomSizes = sizes.OrderBy(x => Guid.NewGuid()).Take(new Random().Next(1, 4));
                foreach (var size in randomSizes)
                {
                    product.ProductSizes.Add(new ProductSize
                    {
                        Product = product,
                        SizeId = size.Id,
                        StockQuantity = new Random().Next(0, 100)
                    });
                }

                var createdProduct = await _productRepository.AddAsync(product);
                var responseDto = await _productRepository.GetProductDtoByIdAsync(createdProduct.Id);
                if (responseDto != null)
                {
                    results.Add(responseDto);
                }
            }

            _logger.LogInformation("Successfully seeded {Count} products", results.Count);
            return Result<List<ProductResponseDto>>.Success(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding {Count} products", count);
            return Result<List<ProductResponseDto>>.Failure("An error occurred while seeding products");
        }
    }
}