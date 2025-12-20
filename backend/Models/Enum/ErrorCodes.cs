// Scheme for HTTP response error codes == "XXXX-YY/ZZZ-NAME" where: 
// XXXS - Module code
// YY  - Error type
// ZZZ  - Specific error code


// Error Types: 10 - Success; 20 - Warning; 30 - Error; 40 - Critical
// Specific error codes 

using Microsoft.EntityFrameworkCore.Diagnostics;

public static class ErrorCodes
{
  public static class Auth
  {
    public const string Success = "AUTH-10/001_SUCCESS";
    public const string ValidToken = "AUTH-10/002_VALID_TOKEN";
    public const string InvalidCredentials = "AUTH-20/010_INVALID_CREDENTIALS";
    public const string InvalidNameIdentifier = "AUTH-20/011_INVALID_NAME_IDENTIFIER";
    public const string TokenExpired = "AUTH-20/020_TOKEN_EXPIRED";
    public const string InvalidToken = "AUTH-20/021_INVALID_TOKEN";
    public const string NotFound = "AUTH-20/030_NOT_FOUND";
    public const string KeyNotFound = "AUTH-20/031_KEY_NOT_FOUND";
    public const string UnauthorizedRole = "AUTH-20/032_UNAUTHORIZED_ROLE";
    public const string TokenTampered = "AUTH-30/040_TOKEN_TAMPERED";
    public const string ForbiddenAccess = "AUTH-30/050_FORBIDDEN_ACCESS";
  }

  public static class Firewall
  {
    public const string IpBlocked = "FIRE-30/001_IP_BLOCKED";
    public const string RateLimitExceeded = "FIRE-30/002_RATE_LIMIT_EXCEEDED";
    public const string DdosAttackDetected = "FIRE-40/003_DDOS_ATTACK_DETECTED";
    public const string MaliciousPayloadDetected = "FIRE-40/004_MALICIOUS_PAYLOAD_DETECTED";
    public const string SQLInjectionDetected = "FIRE-40/005_SQL_INJECTION_DETECTED";
    public const string XSSAttackDetected = "FIRE-40/006_XSS_ATTACK_DETECTED";
    public const string FirewallDetected = "FIRE-30/007_FIREWALL_DETECTED";
    public const string PayloadBlocked = "FIRE-30/008_PAYLOAD_BLOCKED";
    public const string QueryStringBlocked = "FIRE-30/009_QUERY_STRING_BLOCKED";
  }

  public static class Validation
  {
    public const string MissingRequiredField = "VALID-20/001_MISSING_REQUIRED_FIELD";
    public const string InvalidFormat = "VALID-20/002_INVALID_FORMAT";
    public const string ValueOutOfRange = "VALID-20/003_VALUE_OUT_OF_RANGE";
    public const string DuplicateEntry = "VALID-20/004_DUPLICATE_ENTRY";
    public const string DataTypeMismatch = "VALID-20/005_DATA_TYPE_MISMATCH";
    public const string BadRequest = "VALID-30/006_BAD_REQUEST";
  }

  public static class Database
  {
    public const string ConnectionFailed = "DB-30/001_CONNECTION_FAILED";
    public const string QueryTimeout = "DB-30/002_QUERY_TIMEOUT";
    public const string RecordNotFound = "DB-20/003_RECORD_NOT_FOUND";
    public const string ConstraintViolation = "DB-30/004_CONSTRAINT_VIOLATION";
    public const string TransactionFailed = "DB-40/005_TRANSACTION_FAILED";
  }

  public static class Payment
  {
    public const string PaymentSuccessful = "PAY-10/001_PAYMENT_SUCCESSFUL";
    public const string InsufficientFunds = "PAY-20/002_INSUFFICIENT_FUNDS";
    public const string InvalidCardDetails = "PAY-20/003_INVALID_CARD_DETAILS";
    public const string PaymentGatewayError = "PAY-30/004_PAYMENT_GATEWAY_ERROR";
    public const string CurrencyNotSupported = "PAY-20/005_CURRENCY_NOT_SUPPORTED";
  }

  public static class Network
  {
    public const string NetworkTimeout = "NET-30/001_NETWORK_TIMEOUT";
    public const string HostUnreachable = "NET-30/002_HOST_UNREACHABLE";
    public const string DNSResolutionFailed = "NET-30/003_DNS_RESOLUTION_FAILED";
    public const string ConnectionRefused = "NET-30/004_CONNECTION_REFUSED";
    public const string ProtocolError = "NET-30/005_PROTOCOL_ERROR";
  }
  
  public static class General
  {
    public const string Success = "GEN-10/001_SUCCESS";
    public const string OperationSuccessful = "GEN-10/002_OPERATION_SUCCESSFUL";
    public const string FeatureNotImplemented = "GEN-20/001_FEATURE_NOT_IMPLEMENTED";
    public const string ServerError = "GEN-30/001_SERVER_ERROR";
    public const string UnknownError = "GEN-30/002_UNKNOWN_ERROR";
    public const string ServiceUnavailable = "GEN-40/001_SERVICE_UNAVAILABLE";
    public const string DependencyFailure = "GEN-40/002_DEPENDENCY_FAILURE";
    public const string ConflictError = "GEN-30/003_CONFLICT_ERROR";
    public const string UnsupportedMediaType = "GEN-30/004_UNSUPPORTED_MEDIA_TYPE";
  }
}