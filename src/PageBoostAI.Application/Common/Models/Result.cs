namespace PageBoostAI.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public List<string> Errors { get; } = new();

    private Result(bool isSuccess, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        if (errors is not null) Errors = errors;
    }

    public static Result Success() => new(true);
    public static Result Failure(List<string> errors) => new(false, errors);
    public static Result Failure(string error) => new(false, new List<string> { error });
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public List<string> Errors { get; } = new();

    private Result(bool isSuccess, T? data, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        if (errors is not null) Errors = errors;
    }

    public static Result<T> Success(T data) => new(true, data);
    public static Result<T> Failure(List<string> errors) => new(false, default, errors);
    public static Result<T> Failure(string error) => new(false, default, new List<string> { error });
}
