using System.Net;

public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }

    protected AppException(string message, HttpStatusCode statusCode, string errorCode)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

public class TokenExpiredException : AppException
{
  public TokenExpiredException(string message = "The provided token is expired.")
      : base(message, HttpStatusCode.Unauthorized, HttpStatusCodes.AuthCodes.TokenExpired) { }

}

public class InvalidCredentialsException : AppException
{
  public InvalidCredentialsException(string message = "The provided credentials are invalid.")
      : base(message, HttpStatusCode.Unauthorized, HttpStatusCodes.AuthCodes.InvalidCredentials) { }
}

public class InvalidNameIdentifierException : AppException
{
  public InvalidNameIdentifierException(string message = "The provided name identifier is invalid.")
      : base(message, HttpStatusCode.BadRequest, HttpStatusCodes.AuthCodes.InvalidNameIdentifier) { }
}

public class UnauthorizedAppException : AppException
{
  public UnauthorizedAppException(string message = "Unauthorized access.")
      : base(message, HttpStatusCode.Unauthorized, HttpStatusCodes.AuthCodes.Unauthorized) { }
}
public class UnauthorizedRoleException : AppException
{
  public UnauthorizedRoleException(string message = "The user does not have the required role.")
      : base(message, HttpStatusCode.Forbidden, HttpStatusCodes.AuthCodes.Unauthorized) { }
}

public class TokenTamperedException : AppException
{
  public TokenTamperedException(string message = "The provided token has been tampered with.")
      : base(message, HttpStatusCode.Unauthorized, HttpStatusCodes.AuthCodes.TokenTampered) { }
}

public class NotFoundAppException : AppException
{
  public NotFoundAppException(string message = "The requested resource was not found.")
      : base(message, HttpStatusCode.NotFound, HttpStatusCodes.GeneralCodes.NotFound) { }
}

public class ConflictAppException : AppException
{
  public ConflictAppException(string message = "A conflict occurred with the current state of the resource.")
      : base(message, HttpStatusCode.Conflict, HttpStatusCodes.GeneralCodes.Conflict) { }
}

public class BadRequestAppException : AppException
{
  public BadRequestAppException(string message = "The request was invalid or cannot be served.")
      : base(message, HttpStatusCode.BadRequest, HttpStatusCodes.GeneralCodes.BadRequest) { }
}

public class ForbiddenAppException : AppException
{
  public ForbiddenAppException(string message = "You do not have permission to access this resource.")
      : base(message, HttpStatusCode.Forbidden, HttpStatusCodes.AuthCodes.ForbiddenAccess) { }
}

public class InternalServerAppException : AppException
{
  public InternalServerAppException(string message = "An internal server error occurred.")
      : base(message, HttpStatusCode.InternalServerError, HttpStatusCodes.GeneralCodes.ServerError) { }
}

public class MissingRequiredFieldException : AppException
{
  public MissingRequiredFieldException(string message = "A required field is missing.")
      : base(message, HttpStatusCode.BadRequest, HttpStatusCodes.ValidationCodes.MissingRequiredField) { }
}

public class InvalidFormatException : AppException
{
  public InvalidFormatException(string message = "The provided format is invalid.")
      : base(message, HttpStatusCode.BadRequest, HttpStatusCodes.ValidationCodes.InvalidFormat) { }
}

public class ValueOutOfRangeException : AppException
{
  public ValueOutOfRangeException(string message = "The provided value is out of the acceptable range.")
      : base(message, HttpStatusCode.BadRequest, HttpStatusCodes.ValidationCodes.ValueOutOfRange) { }
}

public class DataTypeMismatchException : AppException
{
  public DataTypeMismatchException(string message = "The provided data type does not match the expected type.")
      : base(message, HttpStatusCode.BadRequest, HttpStatusCodes.ValidationCodes.DataTypeMismatch) { }
}