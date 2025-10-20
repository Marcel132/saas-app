public abstract class AppException : Exception
{
  public int StatusCode { get; }

  protected AppException(string message, int statusCode) : base(message)
  {
    StatusCode = statusCode;
  }
}
public class TokenExpiredException : AppException
{
  public TokenExpiredException(string message = "The provided token is expired.")
      : base(message, 400) { }

}

public class InvalidCredentialsException : AppException
{
  public InvalidCredentialsException(string message = "The provided credentials are invalid.")
      : base(message, 400) { }
}

public class InvalidNameIdentifierException : AppException
{
  public InvalidNameIdentifierException(string message = "The provided name identifier is invalid.")
      : base(message, 400) { }
}

public class UnauthorizedRoleException : AppException
{
  public UnauthorizedRoleException(string message = "The user does not have the required role.")
      : base(message, 401) { }
}

public class TokenTemperedException : AppException
{
  public TokenTemperedException(string message = "The provided token has been tampered with.")
      : base(message, 401) { }
}

public class KeyNotFoundAppException : AppException
{
  public KeyNotFoundAppException(string message = "The specified key was not found.")
      : base(message, 404) { }
}

public class NotFoundAppException : AppException
{
  public NotFoundAppException(string message = "The requested resource was not found.")
      : base(message, 404) { }
}

public class ConflictAppException : AppException
{
  public ConflictAppException(string message = "A conflict occurred with the current state of the resource.")
      : base(message, 409) { }
}

public class BadRequestAppException : AppException
{
  public BadRequestAppException(string message = "The request was invalid or cannot be served.")
      : base(message, 400) { }
}

public class ForbiddenAppException : AppException
{
  public ForbiddenAppException(string message = "You do not have permission to access this resource.")
      : base(message, 403) { }
}

public class InternalServerAppException : AppException
{
  public InternalServerAppException(string message = "An internal server error occurred.")
      : base(message, 500) { }
}

public class MissingRequiredFieldException : AppException
{
  public MissingRequiredFieldException(string message = "A required field is missing.")
      : base(message, 400) { }
}

public class InvalidFormatException : AppException
{
  public InvalidFormatException(string message = "The provided format is invalid.")
      : base(message, 400) { }
}

public class ValueOutOfRangeException : AppException
{
  public ValueOutOfRangeException(string message = "The provided value is out of the acceptable range.")
      : base(message, 400) { }
}

public class DataTypeDismatchException : AppException
{
  public DataTypeDismatchException(string message = "The provided data type does not match the expected type.")
      : base(message, 400) { }
}