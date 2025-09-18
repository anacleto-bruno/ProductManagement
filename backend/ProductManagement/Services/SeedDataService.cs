using Bogus;
using Microsoft.Extensions.Logging;
using ProductManagement.Dtos;
using ProductManagement.Entities;
using ProductManagement.Infrastructure.Repositories.Interfaces;
using ProductManagement.Services.Interfaces;
using System.Diagnostics;

namespace ProductManagement.Services;

public class SeedDataService : ISeedDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SeedDataService> _logger;

    public SeedDataService(IUnitOfWork unitOfWork, ILogger<SeedDataService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SeedResponseDto> SeedDataAsync(int numRows)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting database seeding with {NumRows} products", numRows);

        try
        {
            // Clear any existing data to ensure fresh seeding
            await ClearExistingDataAsync();

            // Generate and seed colors (if not already exist)
            var colors = await SeedColorsAsync();
            _logger.LogInformation("Created/Retrieved {ColorCount} colors", colors.Count);

            // Generate and seed sizes (if not already exist)
            var sizes = await SeedSizesAsync();
            _logger.LogInformation("Created/Retrieved {SizeCount} sizes", sizes.Count);

            // Generate and seed products
            var products = await SeedProductsAsync(numRows);
            _logger.LogInformation("Created {ProductCount} products", products.Count);

            // Create product-color relationships
            var productColorRelations = await SeedProductColorsAsync(products, colors);
            _logger.LogInformation("Created {RelationCount} product-color relationships", productColorRelations);

            // Create product-size relationships
            var productSizeRelations = await SeedProductSizesAsync(products, sizes);
            _logger.LogInformation("Created {RelationCount} product-size relationships", productSizeRelations);

            // Verify data persistence by reading back from database
            var verifyProducts = await _unitOfWork.Products.CountAsync();
            var verifyColors = await _unitOfWork.Colors.CountAsync();
            var verifySizes = await _unitOfWork.Sizes.CountAsync();
            _logger.LogInformation("Data verification - Products: {Products}, Colors: {Colors}, Sizes: {Sizes}",
                verifyProducts, verifyColors, verifySizes);

            stopwatch.Stop();

            _logger.LogInformation("Database seeding completed successfully in {ElapsedTime}ms", stopwatch.ElapsedMilliseconds);

            return new SeedResponseDto
            {
                ProductsCreated = verifyProducts,
                ColorsCreated = verifyColors,
                SizesCreated = verifySizes,
                ProductColorRelationsCreated = productColorRelations,
                ProductSizeRelationsCreated = productSizeRelations,
                ExecutionTime = stopwatch.Elapsed,
                Message = $"Successfully seeded {verifyProducts} products with {verifyColors} colors and {verifySizes} sizes"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
            stopwatch.Stop();

            return new SeedResponseDto
            {
                ExecutionTime = stopwatch.Elapsed,
                Message = $"Seeding failed: {ex.Message}"
            };
        }
    }

    private async Task ClearExistingDataAsync()
    {
        _logger.LogInformation("Clearing existing data for fresh seeding");

        // Note: Clearing data in reverse dependency order
        var productColors = await _unitOfWork.ProductColors.GetAllAsync();
        foreach (var pc in productColors)
        {
            await _unitOfWork.ProductColors.DeleteAsync(pc);
        }

        var productSizes = await _unitOfWork.ProductSizes.GetAllAsync();
        foreach (var ps in productSizes)
        {
            await _unitOfWork.ProductSizes.DeleteAsync(ps);
        }

        var products = await _unitOfWork.Products.GetAllAsync();
        foreach (var product in products)
        {
            await _unitOfWork.Products.DeleteAsync(product);
        }

        var colors = await _unitOfWork.Colors.GetAllAsync();
        foreach (var color in colors)
        {
            await _unitOfWork.Colors.DeleteAsync(color);
        }

        var sizes = await _unitOfWork.Sizes.GetAllAsync();
        foreach (var size in sizes)
        {
            await _unitOfWork.Sizes.DeleteAsync(size);
        }

        // Save the deletions
        var deleteResult = await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Cleared existing data, {RowsAffected} rows affected", deleteResult);
    }

    private async Task<List<Color>> SeedColorsAsync()
    {
        _logger.LogInformation("Starting color seeding");

        var colorFaker = new Faker<Color>()
            .RuleFor(c => c.Name, f => f.Commerce.Color())
            .RuleFor(c => c.HexCode, f => f.Internet.Color());

        // Generate common colors plus some random ones
        var predefinedColors = new List<(string Name, string HexCode)>
        {
            ("Black", "#000000"), ("White", "#FFFFFF"), ("Red", "#FF0000"), ("Blue", "#0000FF"),
            ("Green", "#00FF00"), ("Yellow", "#FFFF00"), ("Orange", "#FFA500"), ("Purple", "#800080"),
            ("Pink", "#FFC0CB"), ("Brown", "#A52A2A"), ("Gray", "#808080"), ("Navy", "#000080"),
            ("Maroon", "#800000"), ("Olive", "#808000"), ("Lime", "#00FF00"), ("Aqua", "#00FFFF"),
            ("Teal", "#008080"), ("Silver", "#C0C0C0"), ("Fuchsia", "#FF00FF"), ("Gold", "#FFD700"),
            ("Coral", "#FF7F50"), ("Salmon", "#FA8072"), ("Khaki", "#F0E68C"), ("Violet", "#EE82EE"),
            ("Turquoise", "#40E0D0"), ("Plum", "#DDA0DD"), ("Chocolate", "#D2691E"), ("SandyBrown", "#F4A460"),
            ("LightBlue", "#ADD8E6"), ("LightGreen", "#90EE90"), ("LightPink", "#FFB6C1"), ("LightGray", "#D3D3D3"),
            ("DarkBlue", "#00008B"), ("DarkGreen", "#006400"), ("DarkRed", "#8B0000"), ("DarkGray", "#A9A9A9"),
            ("Beige", "#F5F5DC"), ("Ivory", "#FFFFF0"), ("Lavender", "#E6E6FA"), ("Mint", "#98FF98"),
            ("Peach", "#FFCBA4"), ("Rose", "#FF66CC"), ("Sky", "#87CEEB"), ("Forest", "#228B22"),
            ("Ocean", "#006994"), ("Sunset", "#FF6347"), ("Emerald", "#50C878"), ("Ruby", "#E0115F"),
            ("Sapphire", "#0F52BA"), ("Amethyst", "#9966CC"), ("Topaz", "#FFC87C"), ("Pearl", "#F0EAD6")
        };

        var colors = new List<Color>();

        // Add predefined colors
        foreach (var (name, hexCode) in predefinedColors)
        {
            colors.Add(new Color { Name = name, HexCode = hexCode });
        }

        // Add some random colors to reach 60+ total
        var randomColors = colorFaker.Generate(10);
        foreach (var color in randomColors)
        {
            // Ensure unique names
            if (!colors.Any(c => c.Name.Equals(color.Name, StringComparison.OrdinalIgnoreCase)))
            {
                colors.Add(color);
            }
        }

        // Save colors to database
        foreach (var color in colors)
        {
            await _unitOfWork.Colors.AddAsync(color);
        }

        var colorSaveResult = await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Color seeding completed, {RowsAffected} rows affected", colorSaveResult);

        return colors;
    }

    private async Task<List<Size>> SeedSizesAsync()
    {
        _logger.LogInformation("Starting size seeding");

        var sizes = new List<Size>
        {
            // Clothing sizes
            new Size { Name = "Extra Small", Value = "XS" },
            new Size { Name = "Small", Value = "S" },
            new Size { Name = "Medium", Value = "M" },
            new Size { Name = "Large", Value = "L" },
            new Size { Name = "Extra Large", Value = "XL" },
            new Size { Name = "Double Extra Large", Value = "XXL" },
            new Size { Name = "Triple Extra Large", Value = "XXXL" },

            // Numeric sizes for shoes (US)
            new Size { Name = "Size 6", Value = "6" },
            new Size { Name = "Size 6.5", Value = "6.5" },
            new Size { Name = "Size 7", Value = "7" },
            new Size { Name = "Size 7.5", Value = "7.5" },
            new Size { Name = "Size 8", Value = "8" },
            new Size { Name = "Size 8.5", Value = "8.5" },
            new Size { Name = "Size 9", Value = "9" },
            new Size { Name = "Size 9.5", Value = "9.5" },
            new Size { Name = "Size 10", Value = "10" },
            new Size { Name = "Size 10.5", Value = "10.5" },
            new Size { Name = "Size 11", Value = "11" },
            new Size { Name = "Size 11.5", Value = "11.5" },
            new Size { Name = "Size 12", Value = "12" },
            new Size { Name = "Size 13", Value = "13" },
            new Size { Name = "Size 14", Value = "14" },

            // European clothing sizes
            new Size { Name = "EU 32", Value = "32" },
            new Size { Name = "EU 34", Value = "34" },
            new Size { Name = "EU 36", Value = "36" },
            new Size { Name = "EU 38", Value = "38" },
            new Size { Name = "EU 40", Value = "40" },
            new Size { Name = "EU 42", Value = "42" },
            new Size { Name = "EU 44", Value = "44" },
            new Size { Name = "EU 46", Value = "46" },

            // One size fits all
            new Size { Name = "One Size", Value = "OS" },
            new Size { Name = "One Size Fits All", Value = "OSFA" },

            // Special sizes
            new Size { Name = "Petite", Value = "P" },
            new Size { Name = "Plus", Value = "+" },
            new Size { Name = "Tall", Value = "T" }
        };

        // Save sizes to database
        foreach (var size in sizes)
        {
            await _unitOfWork.Sizes.AddAsync(size);
        }

        var sizeSaveResult = await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Size seeding completed, {RowsAffected} rows affected", sizeSaveResult);

        return sizes;
    }

    private async Task<List<Product>> SeedProductsAsync(int numRows)
    {
        _logger.LogInformation("Starting product seeding for {NumRows} products", numRows);

        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Brand, f => f.Company.CompanyName())
            .RuleFor(p => p.Sku, f => f.Commerce.Ean13())
            .RuleFor(p => p.Price, f => f.Random.Decimal(5.99m, 999.99m))
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0]);

        var products = new List<Product>();
        var existingSkus = new HashSet<string>();

        for (int i = 0; i < numRows; i++)
        {
            Product product;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                product = productFaker.Generate();
                attempts++;
            } while (existingSkus.Contains(product.Sku) && attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                // Generate a unique SKU if faker fails
                product.Sku = $"SKU-{Guid.NewGuid():N}".Substring(0, 20);
            }

            existingSkus.Add(product.Sku);
            products.Add(product);

            await _unitOfWork.Products.AddAsync(product);
        }

        var productSaveResult = await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Product seeding completed, {RowsAffected} rows affected", productSaveResult);

        return products;
    }

    private async Task<int> SeedProductColorsAsync(List<Product> products, List<Color> colors)
    {
        _logger.LogInformation("Starting product-color relationship seeding");

        var random = new Random();
        var relationCount = 0;

        foreach (var product in products)
        {
            // Each product gets 1-4 random colors
            var numColors = random.Next(1, 5);
            var selectedColors = colors.OrderBy(x => random.Next()).Take(numColors).ToList();

            foreach (var color in selectedColors)
            {
                var productColor = new ProductColor
                {
                    ProductId = product.Id,
                    ColorId = color.Id
                };

                await _unitOfWork.ProductColors.AddAsync(productColor);
                relationCount++;
            }
        }

        var relationSaveResult = await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Product-color relationship seeding completed, {RowsAffected} rows affected", relationSaveResult);

        return relationCount;
    }

    private async Task<int> SeedProductSizesAsync(List<Product> products, List<Size> sizes)
    {
        _logger.LogInformation("Starting product-size relationship seeding");

        var random = new Random();
        var relationCount = 0;

        foreach (var product in products)
        {
            // Each product gets 2-6 random sizes
            var numSizes = random.Next(2, 7);
            var selectedSizes = sizes.OrderBy(x => random.Next()).Take(numSizes).ToList();

            foreach (var size in selectedSizes)
            {
                var productSize = new ProductSize
                {
                    ProductId = product.Id,
                    SizeId = size.Id
                };

                await _unitOfWork.ProductSizes.AddAsync(productSize);
                relationCount++;
            }
        }

        var relationSaveResult = await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Product-size relationship seeding completed, {RowsAffected} rows affected", relationSaveResult);

        return relationCount;
    }
}