using System.Net;

// TODO: Change message and error code to be more specific for each exception, currently they are very generic and not informative enough. Maybe add more properties to the exceptions if needed (e.g. for validation errors, include the name of the field that caused the error, etc.)
public abstract class AppException : Exception
{
    public string DomainError { get; }

    protected AppException( string domainError, string? message = null)
        : base(message)
    {
        DomainError = domainError;
    }
}
// -----------
// SCHEMA: {nameOfException}AppException
// -----------

public sealed class UnauthorizedAppException : AppException
{
  public UnauthorizedAppException() : base (
    DomainErrorCodes.AuthCodes.Unauthorized,
    $"Unauthorized access. {DomainErrorCodes.AuthCodes.Unauthorized.Split("_")[0]}"
  ) {}
}
public sealed class InvalidCredentialsAppException : AppException
{
  public InvalidCredentialsAppException() : base (
    DomainErrorCodes.AuthCodes.InvalidCredentials,
    $"Invalid credentials. {DomainErrorCodes.AuthCodes.InvalidCredentials.Split("_")[0]}"
  ) {}
}
public sealed class InvalidNameIdentifierAppException : AppException
{
  public InvalidNameIdentifierAppException() : base (
    DomainErrorCodes.AuthCodes.InvalidNameIdentifier,
    $"Invalid name identifier. {DomainErrorCodes.AuthCodes.InvalidNameIdentifier.Split("_")[0]}"
  ) {}
}
public sealed class TokenExpiredAppException : AppException
{
  public TokenExpiredAppException() : base (
    DomainErrorCodes.AuthCodes.TokenExpired,
    $"Token expired. {DomainErrorCodes.AuthCodes.TokenExpired.Split("_")[0]}"
  ) {}
}
public sealed class TokenTamperedAppException : AppException
{
  public TokenTamperedAppException() : base (
    DomainErrorCodes.AuthCodes.TokenTampered,
    $"Token tampered. {DomainErrorCodes.AuthCodes.TokenTampered.Split("_")[0]}"
  ) {}
}
public sealed class NotFoundAppException : AppException
{
  public NotFoundAppException() : base (
    DomainErrorCodes.GeneralCodes.NotFound,
    $"Resource not found. {DomainErrorCodes.GeneralCodes.NotFound.Split("_")[0]}"
  ) {}
}
public sealed class ConflictAppException : AppException
{
  public ConflictAppException() : base (
    DomainErrorCodes.GeneralCodes.Conflict,
    $"Conflict detected. {DomainErrorCodes.GeneralCodes.Conflict.Split("_")[0]}"
  ) {}
}
public sealed class BadRequestAppException : AppException
{
  public BadRequestAppException() : base (
    DomainErrorCodes.GeneralCodes.BadRequest,
    $"Bad request. {DomainErrorCodes.GeneralCodes.BadRequest.Split("_")[0]}"
  ) {}
}
public sealed class TokenNotFoundAppException : AppException
{
  public TokenNotFoundAppException() : base (
    DomainErrorCodes.AuthCodes.TokenNotFound,
    $"Token not found. {DomainErrorCodes.AuthCodes.TokenNotFound.Split("_")[0]}"
  ) {}
}
public sealed class ForbiddenAppException : AppException
{
  public ForbiddenAppException() : base (
    DomainErrorCodes.AuthCodes.ForbiddenAccess,
    $"Forbidden access. {DomainErrorCodes.AuthCodes.ForbiddenAccess.Split("_")[0]}"
  ) {}
}
public sealed class AccountBlockedAppException : AppException
{
  public AccountBlockedAppException() : base (
    DomainErrorCodes.AuthCodes.AccountBlocked,
    $"Account blocked. {DomainErrorCodes.AuthCodes.AccountBlocked.Split("_")[0]}"
  ) {}
}
public sealed class MissingRequiredFieldAppException : AppException
{
  public MissingRequiredFieldAppException() : base (
    DomainErrorCodes.ValidationCodes.MissingRequiredField,
    $"Missing required field. {DomainErrorCodes.ValidationCodes.MissingRequiredField.Split("_")[0]}"
  ) {}
}
public sealed class InternalServerAppException : AppException
{
  public InternalServerAppException() : base (
    DomainErrorCodes.GeneralCodes.ServerError,
    $"Internal server error. {DomainErrorCodes.GeneralCodes.ServerError.Split("_")[0]}"
  ) {}
}
public sealed class InvalidFormatAppException : AppException
{
  public InvalidFormatAppException() : base (
    DomainErrorCodes.ValidationCodes.InvalidFormat,
    $"Invalid format. {DomainErrorCodes.ValidationCodes.InvalidFormat.Split("_")[0]}"
  ) {}
}
public sealed class ValueOutOfRangeAppException : AppException
{
  public ValueOutOfRangeAppException() : base (
    DomainErrorCodes.ValidationCodes.ValueOutOfRange,
    $"Value out of range. {DomainErrorCodes.ValidationCodes.ValueOutOfRange.Split("_")[0]}"
  ) {}
}

public sealed class SessionNotFoundAppException : AppException
{
  public SessionNotFoundAppException() : base (
    DomainErrorCodes.AuthCodes.SessionNotFound,
    $"Session not found. {DomainErrorCodes.AuthCodes.SessionNotFound.Split("_")[0]}"
  ) {}
}

public sealed class SuspiciousActivityAppException : AppException
{
  public SuspiciousActivityAppException() : base (
    DomainErrorCodes.FirewallCodes.SuspiciousActivityDetected,
    $"Suspicious activity detected. {DomainErrorCodes.FirewallCodes.SuspiciousActivityDetected.Split("_")[0]}"
  ) {}
}
public sealed class InvalidOperationAppException : AppException
{
  public InvalidOperationAppException() : base (
    DomainErrorCodes.GeneralCodes.BadRequest,
    $"Invalid operation. {DomainErrorCodes.GeneralCodes.BadRequest.Split("_")[0]}"
  ) {}
}
