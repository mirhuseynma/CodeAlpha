namespace LinkForge.Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public bool Succeeded { get; init; }
    public string[] Errors { get; init; }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
    
    public static Result Failure(string error)
    {
        return new Result(false, new[] { error });
    }
}

public class Result<T> : Result
{
    public T? Data { get; init; }

    internal Result(bool succeeded, IEnumerable<string> errors, T? data) 
        : base(succeeded, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, Array.Empty<string>(), data);
    }

    public new static Result<T> Failure(IEnumerable<string> errors)
    {
        return new Result<T>(false, errors, default);
    }
    
    public new static Result<T> Failure(string error)
    {
        return new Result<T>(false, new[] { error }, default);
    }
}
