// MODULE-TYPE/NUMBER_NAME

// MODULE
// -------
// GEN     General
// AUTH    Authentication
// USER    User
// VALID   Validation
// FIRE    Firewall
// CONTR   Contract
// APP     Application
// REPORT  Report

// TYPE
// ----
// 10  Success
// 20  Warning
// 30  Business / Validation
// 40  Authorization / Security
// 50  Infrastructure

namespace backend.Api.Http;

public static class DomainCodes
{
  public static class General
  {
    public const string Success = "GEN-10/001_SUCCESS";
    public const string Created = "GEN-10/002_CREATED";

    public const string FeatureNotImplemented = "GEN-20/001_FEATURE_NOT_IMPLEMENTED";

    public const string BadRequest = "GEN-30/001_BAD_REQUEST";
    public const string NotFound = "GEN-30/002_NOT_FOUND";
    public const string Conflict = "GEN-30/003_CONFLICT";

    public const string InternalServerError = "GEN-50/001_INTERNAL_SERVER_ERROR";
    public const string DatabaseUnavailable = "GEN-50/002_DATABASE_UNAVAILABLE";
  }
  public static class Auth
  {
    public const string LoginSucceeded = "AUTH-10/001_LOGIN_SUCCEEDED";
    public const string LogoutSucceeded = "AUTH-10/002_LOGOUT_SUCCEEDED";
    public const string TokenValidated = "AUTH-10/003_TOKEN_VALIDATED";
    public const string RefreshSucceeded = "AUTH-10/004_REFRESH_SUCCEEDED";

    public const string InvalidCredentials = "AUTH-30/001_INVALID_CREDENTIALS";
    public const string SessionNotFound = "AUTH-30/002_SESSION_NOT_FOUND";

    public const string Unauthorized = "AUTH-40/001_UNAUTHORIZED";
    public const string Forbidden = "AUTH-40/002_FORBIDDEN";
    public const string InvalidToken = "AUTH-40/010_INVALID_TOKEN";
    public const string TokenExpired = "AUTH-40/011_TOKEN_EXPIRED";
    public const string TokenNotFound = "AUTH-40/012_TOKEN_NOT_FOUND";
    public const string TokenTampered = "AUTH-40/013_TOKEN_TAMPERED";
    public const string AccountBlocked = "AUTH-40/003_ACCOUNT_BLOCKED";
  }

  public static class Validation
  {
    public const string ValidationSucceeded = "VALID-10/001_VALIDATION_SUCCEEDED";

    public const string InvalidFormat = "VALID-30/001_INVALID_FORMAT";
    public const string MissingRequiredField = "VALID-30/002_MISSING_REQUIRED_FIELD";
    public const string InvalidValue = "VALID-30/003_INVALID_VALUE";
    public const string DuplicateValue = "VALID-30/004_DUPLICATE_VALUE";
    public const string PayloadTooLarge = "VALID-30/005_PAYLOAD_TOO_LARGE";
    public const string UnsupportedMediaType = "VALID-30/006_UNSUPPORTED_MEDIA_TYPE";
    public const string ValueOutOfRange = "VALID-30/007_VALUE_OUT_OF_RANGE";

  }

  public static class User
  {
    public const string Created = "USER-10/001_CREATED";
    public const string Updated = "USER-10/002_UPDATED";
    public const string Deleted = "USER-10/003_DELETED";
    public const string Retrieved = "USER-10/004_RETRIEVED";

    public const string NotFound = "USER-30/001_NOT_FOUND";
    public const string AlreadyExists = "USER-30/002_ALREADY_EXISTS";
    public const string InvalidIdentifier = "USER-30/030_INVALID_IDENTIFIER";
    public const string InvalidNameIdentifier = "USER-30/031_INVALID_NAME_IDENTIFIER";
  }

  public static class Firewall
  {
    public const string IpBlocked = "FIRE-40/001_IP_BLOCKED";
    public const string RateLimitExceeded = "FIRE-40/002_RATE_LIMIT_EXCEEDED";
    public const string QueryBlocked = "FIRE-40/003_QUERY_BLOCKED";
    public const string HeaderBlocked = "FIRE-40/004_HEADER_BLOCKED";
    public const string PayloadBlocked = "FIRE-40/005_PAYLOAD_BLOCKED";

    public const string SqlInjectionDetected = "FIRE-40/006_SQL_INJECTION_DETECTED";
    public const string XssDetected = "FIRE-40/007_XSS_DETECTED";
    public const string SuspiciousActivityDetected = "FIRE-40/008_SUSPICIOUS_ACTIVITY_DETECTED";
    public const string DdosDetected = "FIRE-40/009_DDOS_DETECTED";
  }
}