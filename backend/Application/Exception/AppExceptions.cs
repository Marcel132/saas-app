using backend.Api.Http;

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
    DomainCodes.Auth.Unauthorized,
    message ?? $"Unauthorized access. {DomainCodes.Auth.Unauthorized.Split("_")[0]}"
  ) {}
}
public sealed class InvalidCredentialsAppException : AppException
{
  public InvalidCredentialsAppException(string? message = null) : base (
    DomainCodes.Auth.InvalidCredentials,
    message ?? $"Invalid credentials. {DomainCodes.Auth.InvalidCredentials.Split("_")[0]}"
  ) {}
}
public sealed class InvalidNameIdentifierAppException : AppException
{
  public InvalidNameIdentifierAppException(string? message = null) : base (
    DomainCodes.User.InvalidNameIdentifier,
    message ?? $"Invalid name identifier. {DomainCodes.User.InvalidNameIdentifier.Split("_")[0]}"
  ) {}
}
public sealed class TokenExpiredAppException : AppException
{
  public TokenExpiredAppException(string? message = null) : base (
    DomainCodes.Auth.TokenExpired,
    message ?? $"Token expired. {DomainCodes.Auth.TokenExpired.Split("_")[0]}"
  ) {}
}
public sealed class TokenTamperedAppException : AppException
{
  public TokenTamperedAppException(string? message = null) : base (
    DomainCodes.Auth.TokenTampered,
    message ?? $"Token tampered. {DomainCodes.Auth.TokenTampered.Split("_")[0]}"
  ) {}
}
public sealed class NotFoundAppException : AppException
{
  public NotFoundAppException(string? message = null) : base (
    DomainCodes.General.NotFound,
    message ?? $"Resource not found. {DomainCodes.General.NotFound.Split("_")[0]}"
  ) {}
}
public sealed class ConflictAppException : AppException
{
  public ConflictAppException(string? message = null) : base (
    DomainCodes.General.Conflict,
    message ?? $"Conflict detected. {DomainCodes.General.Conflict.Split("_")[0]}"
  ) {}
}
public sealed class BadRequestAppException : AppException
{
  public BadRequestAppException(string? message = null) : base (
    DomainCodes.General.BadRequest,
    message ?? $"Bad request. {DomainCodes.General.BadRequest.Split("_")[0]}"
  ) {}
}
public sealed class TokenNotFoundAppException : AppException
{
  public TokenNotFoundAppException(string? message = null) : base (
    DomainCodes.Auth.TokenNotFound,
    message ?? $"Token not found. {DomainCodes.Auth.TokenNotFound.Split("_")[0]}"
  ) {}
}
public sealed class ForbiddenAppException : AppException
{
  public ForbiddenAppException(string? message = null) : base (
    DomainCodes.Auth.Forbidden,
    message ?? $"Forbidden access. {DomainCodes.Auth.Forbidden.Split("_")[0]}"
  ) {}
}
public sealed class AccountBlockedAppException : AppException
{
  public AccountBlockedAppException(string? message = null) : base (
    DomainCodes.Auth.AccountBlocked,
    message ?? $"Account blocked. {DomainCodes.Auth.AccountBlocked.Split("_")[0]}"
  ) {}
}
public sealed class MissingRequiredFieldAppException : AppException
{
  public MissingRequiredFieldAppException(string? message = null) : base (
    DomainCodes.Validation.MissingRequiredField,
    message ?? $"Missing required field. {DomainCodes.Validation.MissingRequiredField.Split("_")[0]}"
  ) {}
}
public sealed class InternalServerAppException : AppException
{
  public InternalServerAppException(string? message = null) : base (
    DomainCodes.General.InternalServerError,
    message ?? $"Internal server error. {DomainCodes.General.InternalServerError.Split("_")[0]}"
  ) {}
}
public sealed class InvalidFormatAppException : AppException
{
  public InvalidFormatAppException(string? message = null) : base (
    DomainCodes.Validation.InvalidFormat,
    message ?? $"Invalid format. {DomainCodes.Validation.InvalidFormat.Split("_")[0]}"
  ) {}
}
public sealed class ValueOutOfRangeAppException : AppException
{
  public ValueOutOfRangeAppException(string? message = null) : base (
    DomainCodes.Validation.ValueOutOfRange,
    message ?? $"Value out of range. {DomainCodes.Validation.ValueOutOfRange.Split("_")[0]}"
  ) {}
}

public sealed class SessionNotFoundAppException : AppException
{
  public SessionNotFoundAppException(string? message = null) : base (
    DomainCodes.Auth.SessionNotFound,
    message ?? $"Session not found. {DomainCodes.Auth.SessionNotFound.Split("_")[0]}"
  ) {}
}

public sealed class SuspiciousActivityAppException : AppException
{
  public SuspiciousActivityAppException(string? message = null) : base (
    DomainCodes.Firewall.SuspiciousActivityDetected,
    message ?? $"Suspicious activity detected. {DomainCodes.Firewall.SuspiciousActivityDetected.Split("_")[0]}"
  ) {}
}
public sealed class InvalidOperationAppException : AppException
{
  public InvalidOperationAppException(string? message = null) : base (
    DomainCodes.General.BadRequest,
    message ?? $"Invalid operation. {DomainCodes.General.BadRequest.Split("_")[0]}"
  ) {}
}
