// Scheme for HTTP response error codes == "XXXX-YY/ZZZ-NAME" where: 
// XXXX - Module code
// YY  - Error type
// ZZZ  - Specific error code


// Error Types: 10 - Success; 20 - Soft warning / hint (fe. deprecated, fallback); 30 - Client / business error; 40 - Security / system / critical
// Specific error codes 

using Microsoft.EntityFrameworkCore.Diagnostics;

public static class HttpStatusCodes
{
  public static class GeneralCodes
  {
    public const string Success = "GEN-10/001_SUCCESS";
    public const string OperationSuccessful = "GEN-10/002_OPERATION_SUCCESSFUL";
    public const string FeatureNotImplemented = "GEN-20/001_FEATURE_NOT_IMPLEMENTED";
    public const string ServerError = "GEN-30/001_SERVER_ERROR";
    public const string Conflict = "GEN-30/002_CONFLICT_ERROR";
    public const string BadRequest = "GEN-30/003_BAD_REQUEST";
    public const string UnsupportedMediaType = "GEN-30/004_UNSUPPORTED_MEDIA_TYPE";
    public const string NotFound = "GEN-30/005_NOT_FOUND";
    public const string ServiceUnavailable = "GEN-40/001_SERVICE_UNAVAILABLE";
    public const string DependencyFailure = "GEN-40/002_DEPENDENCY_FAILURE";
  }
  public static class AuthCodes
  {
    public const string Success = "AUTH-10/001_SUCCESS";
    public const string ValidToken = "AUTH-10/002_VALID_TOKEN";
    public const string ValidCredentials = "AUTH-10/003_VALID_CREDENTIALS";
    public const string Authorized = "AUTH-10/004_AUTHORIZED";
    public const string InvalidCredentials = "AUTH-20/002_INVALID_CREDENTIALS";
    public const string InvalidNameIdentifier = "AUTH-20/003_INVALID_NAME_IDENTIFIER";
    public const string TokenExpired = "AUTH-30/001_TOKEN_EXPIRED";
    public const string Unauthorized = "AUTH-30/002_UNAUTHORIZED";
    public const string ForbiddenAccess = "AUTH-30/003_FORBIDDEN_ACCESS";
    public const string InvalidToken = "AUTH-30/004_INVALID_TOKEN";
    public const string BadRequest = "AUTH-30/005_BAD_REQUEST";
    public const string TokenTampered = "AUTH-40/001_TOKEN_TAMPERED";
    public const string MultipleFailedLogins = "AUTH-40/002_MULTIPLE_FAILED_LOGINS";
    public const string AccountBlocked = "AUTH-40/003_ACCOUNT_BLOCKED";
  }

  public static class ValidationCodes
  {
    public const string ValidData = "VALID-10/001_VALID_DATA";
    public const string DataTypeMismatch = "VALID-20/001_DATA_TYPE_MISMATCH";
    public const string MissingRequiredField = "VALID-30/001_MISSING_REQUIRED_FIELD";
    public const string InvalidFormat = "VALID-30/002_INVALID_FORMAT";
    public const string ValueOutOfRange = "VALID-30/003_VALUE_OUT_OF_RANGE";
    public const string DuplicateEntry = "VALID-30/004_DUPLICATE_ENTRY";
    public const string BadRequest = "VALID-30/005_BAD_REQUEST";
    public const string ValidationFailed = "VALID-30/006_VALIDATION_FAILED";
    public const string PayloadTooLarge = "VALID-30/007_PAYLOAD_TOO_LARGE";
    public const string InvalidModel = "VALID-30/008_INVALID_MODEL";
  }

  public static class UserCodes
  {
    public const string UserCreated = "USER-10/001_USER_CREATED";
    public const string UserUpdated = "USER-10/002_USER_UPDATED";
    public const string UserDeleted = "USER-10/003_USER_DELETED";
    public const string UserReadCorrectly = "USER-10/004_USER_READ_CORRECTLY";
    public const string InvalidUserId = "USER-20/003_INVALID_USER_ID";
    public const string UserCreationFailed = "USER-30/001_USER_CREATION_FAILED";
    public const string UserUpdateFailed = "USER-30/002_USER_UPDATE_FAILED";
    public const string UserDeletionFailed = "USER-30/003_USER_DELETION_FAILED";
    public const string UserNotFound = "USER-30/004_USER_NOT_FOUND";
    public const string UserAlreadyExists = "USER-30/005_USER_ALREADY_EXISTS";
  } 

  public static class FirewallCodes
  {
    public const string IpBlocked = "FIRE-30/001_IP_BLOCKED";
    public const string RateLimitExceeded = "FIRE-30/002_RATE_LIMIT_EXCEEDED";
    public const string QueryStringBlocked = "FIRE-30/003_QUERY_STRING_BLOCKED";
    public const string HeadersBlocked = "FIRE-30/004_HEADERS_BLOCKED";
    public const string InvalidFormatDetected = "FIRE-30/005_INVALID_FORMAT_DETECTED";
    public const string PayloadBlocked = "FIRE-30/006_PAYLOAD_BLOCKED";
    public const string DdosAttackDetected = "FIRE-40/001_DDOS_ATTACK_DETECTED";
    public const string SuspiciousActivityDetected = "FIRE-40/002_SUSPICIOUS_ACTIVITY_DETECTED";
    public const string SuspiciousPayloadDetected = "FIRE-40/003_SUSPICIOUS_PAYLOAD_DETECTED";
    public const string SQLInjectionDetected = "FIRE-40/004_SQL_INJECTION_DETECTED";
    public const string XSSAttackDetected = "FIRE-40/005_XSS_ATTACK_DETECTED";
  }
}