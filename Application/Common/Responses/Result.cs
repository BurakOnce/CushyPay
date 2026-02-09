namespace CushyPay.Application.Common.Responses;

public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    protected Result(bool isSuccess, string? error = null, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? new List<string>();
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(List<string> errors) => new(false, errors: errors);
}

public class Result<T> : Result
{
    public T? Value { get; private set; }

    private Result(T value, bool isSuccess, string? error = null, List<string>? errors = null)
        : base(isSuccess, error, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true);
    public static new Result<T> Failure(string error) => new(default!, false, error);
    public static new Result<T> Failure(List<string> errors) => new(default!, false, errors: errors);
}

