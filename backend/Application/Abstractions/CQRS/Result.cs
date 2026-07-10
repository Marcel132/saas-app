namespace backend.Application.Abstractions.CQRS;

public sealed class Error
{
  public string Code { get; }
  public string Message { get; }
  public HttpResponseState State { get; }

  public Error(string code, string message, HttpResponseState state)
  {
    Code = code;
    Message = message;
    State = state;
  }

  public static readonly Error None = new(string.Empty, string.Empty, HttpResponseState.Success);
}

public class Result
{
  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public Error Error { get; }

  protected Result(bool isSuccess, Error error)
  {
    if (isSuccess && error != Error.None)
      throw new InvalidOperationException("Sukces nie może mieć przypisanego błędu.");
    if (!isSuccess && error == Error.None)
      throw new InvalidOperationException("Porażka musi mieć przypisany błąd.");

    IsSuccess = isSuccess;
    Error = error;
  }

  public static Result Success() => new(true, Error.None);
  public static Result Failure(Error error) => new(false, error);
}

public sealed class Result<TValue> : Result
{
  private readonly TValue? _value;

  public TValue Value => IsSuccess
    ? _value!
    : throw new InvalidOperationException("Nie można pobrać wartości z nieudanego Result.");

  private Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
  {
    _value = value;
  }

  public static Result<TValue> Success(TValue value) => new(value, true, Error.None);
  public static new Result<TValue> Failure(Error error) => new(default, false, error);

  public static implicit operator Result<TValue>(TValue value) => Success(value);
}