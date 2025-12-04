namespace ServiceCommons;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ErrorType? ErrorType { get; }
    public object? Value { get; }
    public string? Error { get; }

    protected Result(bool isSuccess, string? error, object? value = null, ErrorType? errorType = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public static Result Success()
        => new Result(true, null);
    
    public static Result Success(object? value)
        => new Result(true, null, value);

    public static Result Failure(string error, ErrorType? errorType = null)
        => new Result(false, error, errorType);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error, ErrorType? errorType = null)
        : base(isSuccess, error, errorType)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
        => new Result<T>(true, value, null);

    public new static Result<T> Failure(string error, ErrorType? errorType = null)
        => new Result<T>(false, default, error, errorType);
    
    public new static Result<T> Failure(string error, T value, ErrorType? errorType = null)
        => new Result<T>(false, value, error, errorType);
    
    public new static Result<T> Failure<U>(Result<U> result)
        => new Result<T>(false, default, result.Error, result.ErrorType);

    public static Result<U> Combine<T, U>(Result<T> result, Func<T, U> transform)
    {
        return result.IsFailure 
            ? Result<U>.Failure(result.Error!, result.ErrorType)
            : Result<U>.Success(transform(result.Value!));
    }
}

public enum ErrorType
{
    Forbidden,
    NotFound,
    Conflict,
    Unexpected,
    TooManyAttempts,
    Unauthorized
} 