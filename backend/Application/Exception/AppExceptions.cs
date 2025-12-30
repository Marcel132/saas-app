using System.Net;

public abstract class AppException : Exception
{
    public string ErrorCode { get; }

    protected AppException( string errorCode, string? message = null)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
// -----------
// SCHEMA: {nameOfException}AppException
// -----------

public sealed class UnauthorizedAppException : AppException
{
  public UnauthorizedAppException() : base (DomainErrorCodes.AuthCodes.Unauthorized) {}
}
public sealed class InvalidCredentialsAppException : AppException
{
  public InvalidCredentialsAppException() : base (DomainErrorCodes.AuthCodes.InvalidCredentials) {}
}
public sealed class InvalidNameIdentifierAppException : AppException
{
  public InvalidNameIdentifierAppException() : base (DomainErrorCodes.AuthCodes.InvalidNameIdentifier) {}
}
public sealed class TokenExpiredAppException : AppException
{
  public TokenExpiredAppException() : base (DomainErrorCodes.AuthCodes.TokenExpired) {}
}
public sealed class TokenTamperedAppException : AppException
{
  public TokenTamperedAppException() : base (DomainErrorCodes.AuthCodes.TokenTampered) {}
}
public sealed class NotFoundAppException : AppException
{
  public NotFoundAppException() : base (DomainErrorCodes.GeneralCodes.NotFound) {}
}
public sealed class ConflictAppException : AppException
{
  public ConflictAppException() : base (DomainErrorCodes.GeneralCodes.Conflict) {}
}
public sealed class BadRequestAppException : AppException
{
  public BadRequestAppException() : base (DomainErrorCodes.GeneralCodes.BadRequest) {}
}
public sealed class ForbiddenAppException : AppException
{
  public ForbiddenAppException() : base (DomainErrorCodes.AuthCodes.ForbiddenAccess) {}
}
public sealed class AccountBlockedAppException : AppException
{
  public AccountBlockedAppException() : base (DomainErrorCodes.AuthCodes.AccountBlocked) {}
}
public sealed class MissingRequiredFieldAppException : AppException
{
  public MissingRequiredFieldAppException() : base (DomainErrorCodes.ValidationCodes.MissingRequiredField) {}
}
public sealed class InternalServerAppException : AppException
{
  public InternalServerAppException() : base (DomainErrorCodes.GeneralCodes.ServerError) {}
}
public sealed class InvalidFormatAppException : AppException
{
  public InvalidFormatAppException() : base (DomainErrorCodes.ValidationCodes.InvalidFormat) {}
}
public sealed class ValueOutOfRangeAppException : AppException
{
  public ValueOutOfRangeAppException() : base (DomainErrorCodes.ValidationCodes.ValueOutOfRange) {}
}
