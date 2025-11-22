using SIPRI.Application.Common.Results;
using Xunit;

namespace SIPRI.Application.Tests.Common;

public class ResultTests
{
    #region Result (sem valor) Tests

    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResultWithError()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Constructor_WhenSuccessWithError_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            new TestResult(true, error));
    }

    [Fact]
    public void Constructor_WhenFailureWithoutError_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            new TestResult(false, Error.None));
    }

    #endregion

    #region Result<T> (com valor) Tests

    [Fact]
    public void Success_WithValue_ShouldCreateSuccessResultWithValue()
    {
        // Arrange
        var expectedValue = "Test Value";

        // Act
        var result = Result.Success(expectedValue);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(expectedValue, result.Value);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_Generic_ShouldCreateFailureResult()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");

        // Act
        var result = Result.Failure<string>(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Value_WhenSuccess_ShouldReturnValue()
    {
        // Arrange
        var expectedValue = 42;
        var result = Result.Success(expectedValue);

        // Act
        var value = result.Value;

        // Assert
        Assert.Equal(expectedValue, value);
    }

    [Fact]
    public void Value_WhenFailure_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");
        var result = Result.Failure<int>(error);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Contains("falha", exception.Message.ToLower());
    }

    [Fact]
    public void Success_WithComplexType_ShouldPreserveType()
    {
        // Arrange
        var complexObject = new TestComplexType { Id = 1, Name = "Test" };

        // Act
        var result = Result.Success(complexObject);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(complexObject.Id, result.Value.Id);
        Assert.Equal(complexObject.Name, result.Value.Name);
    }

    [Fact]
    public void Success_WithNullValue_ShouldAllowNull()
    {
        // Act
        var result = Result.Success<string?>(null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    #endregion

    #region Error Tests

    [Fact]
    public void Error_None_ShouldHaveEmptyCodeAndMessage()
    {
        // Act
        var error = Error.None;

        // Assert
        Assert.Equal(string.Empty, error.Code);
        Assert.Equal(string.Empty, error.Message);
    }

    [Fact]
    public void Error_WithCodeAndMessage_ShouldCreateError()
    {
        // Arrange
        var code = "ValidationError";
        var message = "Validation failed";

        // Act
        var error = new Error(code, message);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(message, error.Message);
    }

    [Fact]
    public void Error_Equality_ShouldCompareByValue()
    {
        // Arrange
        var error1 = new Error("Code1", "Message1");
        var error2 = new Error("Code1", "Message1");
        var error3 = new Error("Code2", "Message2");

        // Assert
        Assert.Equal(error1, error2);
        Assert.NotEqual(error1, error3);
    }

    [Fact]
    public void Error_GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var error1 = new Error("Code1", "Message1");
        var error2 = new Error("Code1", "Message1");

        // Assert
        Assert.Equal(error1.GetHashCode(), error2.GetHashCode());
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Result_ChainedOperations_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var successResult = Result.Success();
        var failureResult = Result.Failure(new Error("Error", "Failed"));

        // Assert
        Assert.True(successResult.IsSuccess);
        Assert.True(failureResult.IsFailure);
        Assert.NotEqual(successResult.Error, failureResult.Error);
    }

    [Fact]
    public void ResultWithValue_ChainedOperations_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var successResult = Result.Success(100);
        var failureResult = Result.Failure<int>(new Error("Error", "Failed"));

        // Assert
        Assert.True(successResult.IsSuccess);
        Assert.Equal(100, successResult.Value);
        Assert.True(failureResult.IsFailure);
        Assert.Throws<InvalidOperationException>(() => failureResult.Value);
    }

    #endregion

    // Helper classes for testing
    private class TestResult : Result
    {
        public TestResult(bool isSuccess, Error error) : base(isSuccess, error) { }
    }

    private class TestComplexType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
