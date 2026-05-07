namespace Gold.Application.Common;

public class Result
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static Result Success() => new() { Succeeded = true };
    public static Result Failure(string error) => new() { Succeeded = false, Error = error, Errors = new[] { error } };
    public static Result Failure(IEnumerable<string> errors)
    {
        var list = errors.ToArray();
        return new Result { Succeeded = false, Error = list.FirstOrDefault(), Errors = list };
    }
}

public class Result<T> : Result
{
    public T? Data { get; init; }

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
    public new static Result<T> Failure(string error) => new() { Succeeded = false, Error = error, Errors = new[] { error } };
    public new static Result<T> Failure(IEnumerable<string> errors)
    {
        var list = errors.ToArray();
        return new Result<T> { Succeeded = false, Error = list.FirstOrDefault(), Errors = list };
    }
}
