namespace ProductManagement.models;

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string> Errors { get; init; } = new();

    private Result(bool isSuccess, T? data, string? errorMessage, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, null);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(false, default, errorMessage);
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>(false, default, null, errors);
    }
}

public class Result
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string> Errors { get; init; } = new();

    private Result(bool isSuccess, string? errorMessage, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Errors = errors ?? new List<string>();
    }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(string errorMessage)
    {
        return new Result(false, errorMessage);
    }

    public static Result Failure(List<string> errors)
    {
        return new Result(false, null, errors);
    }
}