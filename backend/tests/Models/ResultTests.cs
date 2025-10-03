using FluentAssertions;
using ProductManagement.models;

namespace ProductManagement.Tests.Models;

/// <summary>
/// Tests for Result and Result<T> classes
/// </summary>
public class ResultTests
{
    [Fact]
    public void Result_Success_CreatesSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Result_FailureWithMessage_CreatesFailedResult()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Result_FailureWithErrors_CreatesFailedResultWithErrorList()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2", "Error 3" };

        // Act
        var result = Result.Failure(errors);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.Errors.Should().BeEquivalentTo(errors);
        result.Errors.Should().HaveCount(3);
    }

    [Fact]
    public void Result_FailureWithEmptyErrors_CreatesFailedResultWithEmptyErrorList()
    {
        // Arrange
        var errors = new List<string>();

        // Act
        var result = Result.Failure(errors);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ResultT_Success_CreatesSuccessfulResultWithData()
    {
        // Arrange
        var testData = "Test data";

        // Act
        var result = Result<string>.Success(testData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(testData);
        result.ErrorMessage.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ResultT_SuccessWithComplexObject_CreatesSuccessfulResult()
    {
        // Arrange
        var testData = new { Id = 1, Name = "Test Object", Values = new List<int> { 1, 2, 3 } };

        // Act
        var result = Result<object>.Success(testData);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(testData);
        result.ErrorMessage.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ResultT_SuccessWithNull_CreatesSuccessfulResultWithNullData()
    {
        // Act
        var result = Result<string?>.Success(null);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ResultT_FailureWithMessage_CreatesFailedResult()
    {
        // Arrange
        var errorMessage = "Operation failed";

        // Act
        var result = Result<string>.Failure(errorMessage);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ResultT_FailureWithErrors_CreatesFailedResultWithErrorList()
    {
        // Arrange
        var errors = new List<string> { "Validation error 1", "Validation error 2" };

        // Act
        var result = Result<int>.Failure(errors);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(0); // default value for int
        result.ErrorMessage.Should().BeNull();
        result.Errors.Should().BeEquivalentTo(errors);
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void ResultT_FailureWithEmptyErrors_CreatesFailedResultWithEmptyErrorList()
    {
        // Arrange
        var errors = new List<string>();

        // Act
        var result = Result<double>.Failure(errors);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().Be(0.0);
        result.ErrorMessage.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ResultT_FailureWithNullErrors_CreatesFailedResultWithEmptyErrorList()
    {
        // Arrange
        var errorMessage = "Test error";

        // Act
        var result = Result<string>.Failure(errorMessage);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Errors.Should().BeEmpty(); // Should initialize empty list even when null is passed internally
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A very long error message that contains lots of details about what went wrong in the system")]
    public void Result_FailureWithVariousMessages_HandlesCorrectly(string errorMessage)
    {
        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A very long error message for generic result")]
    public void ResultT_FailureWithVariousMessages_HandlesCorrectly(string errorMessage)
    {
        // Act
        var result = Result<bool>.Failure(errorMessage);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeFalse(); // default value for bool
        result.ErrorMessage.Should().Be(errorMessage);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Result_Properties_AreReadOnly()
    {
        // Arrange & Act
        var successResult = Result.Success();
        var failureResult = Result.Failure("Error");

        // Assert - Properties should have init-only setters, meaning they can't be modified after construction
        successResult.IsSuccess.Should().BeTrue();
        successResult.ErrorMessage.Should().BeNull();
        successResult.Errors.Should().BeEmpty();

        failureResult.IsSuccess.Should().BeFalse();
        failureResult.ErrorMessage.Should().Be("Error");
        failureResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ResultT_Properties_AreReadOnly()
    {
        // Arrange & Act
        var successResult = Result<int>.Success(42);
        var failureResult = Result<int>.Failure("Error");

        // Assert - Properties should have init-only setters
        successResult.IsSuccess.Should().BeTrue();
        successResult.Data.Should().Be(42);
        successResult.ErrorMessage.Should().BeNull();
        successResult.Errors.Should().BeEmpty();

        failureResult.IsSuccess.Should().BeFalse();
        failureResult.Data.Should().Be(0);
        failureResult.ErrorMessage.Should().Be("Error");
        failureResult.Errors.Should().BeEmpty();
    }
}