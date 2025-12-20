public enum HttpResponseState
{
  Authorized = 200, 
  Success = 200,
  BadRequest = 400,
  Unauthorized = 401,
  Forbidden = 403,
  FirewallDetected = 403,
  NotFound = 404,
  Conflict = 409,
  ContentTooLarge = 413,
  UnsupportedMediaType = 415,
  ServerError = 500
}