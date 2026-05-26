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
  public UnauthorizedAppException(string? message = null) : base (
    DomainErrorCodes.AuthCodes.Unauthorized,
    message ?? $"Unauthorized access. {DomainErrorCodes.AuthCodes.Unauthorized.Split("_")[0]}"
  ) {}
}
public sealed class InvalidCredentialsAppException : AppException
{
  public InvalidCredentialsAppException(string? message = null) : base (
    DomainErrorCodes.AuthCodes.InvalidCredentials,
    message ?? $"Invalid credentials. {DomainErrorCodes.AuthCodes.InvalidCredentials.Split("_")[0]}"
  ) {}
}
public sealed class InvalidNameIdentifierAppException : AppException
{
  public InvalidNameIdentifierAppException(string? message = null) : base (
    DomainErrorCodes.AuthCodes.InvalidNameIdentifier,
    message ?? $"Invalid name identifier. {DomainErrorCodes.AuthCodes.InvalidNameIdentifier.Split("_")[0]}"
  ) {}
}
public sealed class TokenExpiredAppException : AppException
{
  public TokenExpiredAppException(string? message = null) : base (
    DomainErrorCodes.AuthCodes.TokenExpired,
    message ?? $"Token expired. {DomainErrorCodes.AuthCodes.TokenExpired.Split("_")[0]}"
  ) {}
}
public sealed class TokenTamperedAppException : AppException
{
  public TokenTamperedAppException(string? message = null) : base (
    DomainErrorCodes.AuthCodes.TokenTampered,
    message ?? $"Token tampered. {DomainErrorCodes.AuthCodes.TokenTampered.Split("_")[0]}"
  ) {}
}
public sealed class NotFoundAppException : AppException
{
  public NotFoundAppException(string? message = null) : base (
    DomainErrorCodes.GeneralCodes.NotFound,
    message ?? $"Resource not found. {DomainErrorCodes.GeneralCodes.NotFound.Split("_")[0]}"
  ) {}
}
public sealed class ConflictAppException : AppException
{
  public ConflictAppException(string? message = null) : base (
    DomainErrorCodes.GeneralCodes.Conflict,
    message ?? $"Conflict detected. {DomainErrorCodes.GeneralCodes.Conflict.Split("_")[0]}"
  ) {}
}
public sealed class BadRequestAppException : AppException
{
  public BadRequestAppException(string? message = null) : base (
    DomainErrorCodes.GeneralCodes.BadRequest,
    message ?? $"Bad request. {DomainErrorCodes.GeneralCodes.BadRequest.Split("_")[0]}"
  ) {}
}
public sealed class TokenNotFoundAppException : AppException
{
  public TokenNotFoundAppException(string? message = null) : base (
    DomainErrorCodes.AuthCodes.TokenNotFound,
    message ?? $"Token not found. {DomainErrorCodes.AuthCodes.TokenNotFound.Split("_")[0]}"
  ) {}
}
public sealed class ForbiddenAppException : AppException
{
  public ForbiddenAppException(string? message = null) : base (
    DomainErrorCodes.AuthCodes.ForbiddenAccess,
    message ?? $"Forbidden access. {DomainErrorCodes.AuthCodes.ForbiddenAccess.Split("_")[0]}"
  ) {}
}
public sealed class AccountBlockedAppException : AppException
{
  public AccountBlockedAppException(string? message = null) : base (
    DomainErrorCodes.AuthCodes.AccountBlocked,
    message ?? $"Account blocked. {DomainErrorCodes.AuthCodes.AccountBlocked.Split("_")[0]}"
  ) {}
}
public sealed class MissingRequiredFieldAppException : AppException
{
  public MissingRequiredFieldAppException(string? message = null) : base (
    DomainErrorCodes.ValidationCodes.MissingRequiredField,
    message ?? $"Missing required field. {DomainErrorCodes.ValidationCodes.MissingRequiredField.Split("_")[0]}"
  ) {}
}
public sealed class InternalServerAppException : AppException
{
  public InternalServerAppException(string? message = null) : base (
    DomainErrorCodes.GeneralCodes.ServerError,
    message ?? $"Internal server error. {DomainErrorCodes.GeneralCodes.ServerError.Split("_")[0]}"
  ) {}
}
public sealed class InvalidFormatAppException : AppException
{
  public InvalidFormatAppException(string? message = null) : base (
    DomainErrorCodes.ValidationCodes.InvalidFormat,
    message ?? $"Invalid format. {DomainErrorCodes.ValidationCodes.InvalidFormat.Split("_")[0]}"
  ) {}
}
public sealed class ValueOutOfRangeAppException : AppException
{
  public ValueOutOfRangeAppException(string? message = null) : base (
    DomainErrorCodes.ValidationCodes.ValueOutOfRange,
    message ?? $"Value out of range. {DomainErrorCodes.ValidationCodes.ValueOutOfRange.Split("_")[0]}"
  ) {}
}

public sealed class SessionNotFoundAppException : AppException
{
  public SessionNotFoundAppException(string? message = null) : base (
    DomainErrorCodes.AuthCodes.SessionNotFound,
    message ?? $"Session not found. {DomainErrorCodes.AuthCodes.SessionNotFound.Split("_")[0]}"
  ) {}
}

public sealed class SuspiciousActivityAppException : AppException
{
  public SuspiciousActivityAppException(string? message = null) : base (
    DomainErrorCodes.FirewallCodes.SuspiciousActivityDetected,
    message ?? $"Suspicious activity detected. {DomainErrorCodes.FirewallCodes.SuspiciousActivityDetected.Split("_")[0]}"
  ) {}
}
public sealed class InvalidOperationAppException : AppException
{
  public InvalidOperationAppException(string? message = null) : base (
    DomainErrorCodes.GeneralCodes.BadRequest,
    message ?? $"Invalid operation. {DomainErrorCodes.GeneralCodes.BadRequest.Split("_")[0]}"
  ) {}
}
