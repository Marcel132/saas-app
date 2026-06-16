
public enum HttpResponseState
{
  Success = 200,
  Created = 201,
  NoContent = 204,
  BadRequest = 400,
  Unauthorized = 401,
  Forbidden = 403,
  NotFound = 404,
  Conflict = 409,
  ContentTooLarge = 413,
  UnsupportedMediaType = 415,
  TooManyRequests = 429,
  ServerError = 500
}