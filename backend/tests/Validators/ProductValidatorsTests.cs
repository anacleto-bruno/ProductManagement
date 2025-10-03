using FluentAssertions;
using ProductManagement.validators;
using ProductManagement.dtos;

namespace ProductManagement.Tests.Validators;

public class ProductValidatorsTests
{
    [Fact]
    public void CreateProductRequestValidator_ValidProduct_PassesValidation()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var validProduct = new CreateProductRequestDto
        {
            Name = "Valid Product",
            Description = "A valid product description",
            Model = "Model123",
            Brand = "BrandName",
            Sku = "SKU123456",
            Price = 99.99m,
            Category = "Electronics",
            ColorIds = new List<int> { 1, 2 },
            SizeIds = new List<int> { 1 }
        };

        // Act
        var result = validator.Validate(validProduct);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateProductRequestValidator_InvalidName_FailsValidation(string invalidName)
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var product = new CreateProductRequestDto
        {
            Name = invalidName,
            Model = "Model123",
            Brand = "BrandName",
            Sku = "SKU123456",
            Price = 99.99m
        };

        // Act
        var result = validator.Validate(product);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductRequestDto.Name));
    }

    [Fact]
    public void CreateProductRequestValidator_NullName_FailsValidation()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var product = new CreateProductRequestDto
        {
            Name = null!,
            Model = "Model123",
            Brand = "BrandName",
            Sku = "SKU123456",
            Price = 99.99m
        };

        // Act
        var result = validator.Validate(product);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductRequestDto.Name));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void CreateProductRequestValidator_InvalidPrice_FailsValidation(decimal invalidPrice)
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var product = new CreateProductRequestDto
        {
            Name = "Valid Product",
            Model = "Model123",
            Brand = "BrandName",
            Sku = "SKU123456",
            Price = invalidPrice
        };

        // Act
        var result = validator.Validate(product);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductRequestDto.Price));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateProductRequestValidator_InvalidSku_FailsValidation(string invalidSku)
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var product = new CreateProductRequestDto
        {
            Name = "Valid Product",
            Model = "Model123",
            Brand = "BrandName",
            Sku = invalidSku,
            Price = 99.99m
        };

        // Act
        var result = validator.Validate(product);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductRequestDto.Sku));
    }

    [Fact]
    public void CreateProductRequestValidator_NullSku_FailsValidation()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var product = new CreateProductRequestDto
        {
            Name = "Valid Product",
            Model = "Model123",
            Brand = "BrandName",
            Sku = null!,
            Price = 99.99m
        };

        // Act
        var result = validator.Validate(product);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductRequestDto.Sku));
    }

    [Fact]
    public void UpdateProductRequestValidator_ValidProduct_PassesValidation()
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();
        var validProduct = new UpdateProductRequestDto
        {
            Name = "Updated Product",
            Description = "Updated description",
            Model = "UpdatedModel",
            Brand = "UpdatedBrand",
            Sku = "UPDATED_SKU",
            Price = 149.99m,
            Category = "Updated Category",
            ColorIds = new List<int> { 1 },
            SizeIds = new List<int> { 1, 2 }
        };

        // Act
        var result = validator.Validate(validProduct);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void PaginationRequestValidator_ValidRequest_PassesValidation()
    {
        // Arrange
        var validator = new PaginationRequestValidator();
        var validRequest = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            SearchTerm = "test",
            Category = "Electronics",
            MinPrice = 10.00m,
            MaxPrice = 500.00m,
            SortBy = "name",
            Descending = false
        };

        // Act
        var result = validator.Validate(validRequest);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PaginationRequestValidator_InvalidPage_FailsValidation(int invalidPage)
    {
        // Arrange
        var validator = new PaginationRequestValidator();
        var request = new PaginationRequestDto
        {
            Page = invalidPage,
            PageSize = 20
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PaginationRequestDto.Page));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void PaginationRequestValidator_InvalidPageSize_FailsValidation(int invalidPageSize)
    {
        // Arrange
        var validator = new PaginationRequestValidator();
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = invalidPageSize
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PaginationRequestDto.PageSize));
    }

    [Fact]
    public void PaginationRequestValidator_MinPriceGreaterThanMaxPrice_FailsValidation()
    {
        // Arrange
        var validator = new PaginationRequestValidator();
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            MinPrice = 100.00m,
            MaxPrice = 50.00m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Minimum price must be less than or equal to maximum price"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void PaginationRequestValidator_NegativeMinPrice_FailsValidation(decimal negativePrice)
    {
        // Arrange
        var validator = new PaginationRequestValidator();
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            MinPrice = negativePrice
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PaginationRequestDto.MinPrice));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void PaginationRequestValidator_NegativeMaxPrice_FailsValidation(decimal negativePrice)
    {
        // Arrange
        var validator = new PaginationRequestValidator();
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            MaxPrice = negativePrice
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PaginationRequestDto.MaxPrice));
    }

    [Fact]
    public void PaginationRequestValidator_ValidPriceRange_PassesValidation()
    {
        // Arrange
        var validator = new PaginationRequestValidator();
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            MinPrice = 10.00m,
            MaxPrice = 100.00m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void PaginationRequestValidator_EqualMinMaxPrice_PassesValidation()
    {
        // Arrange
        var validator = new PaginationRequestValidator();
        var request = new PaginationRequestDto
        {
            Page = 1,
            PageSize = 20,
            MinPrice = 50.00m,
            MaxPrice = 50.00m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}