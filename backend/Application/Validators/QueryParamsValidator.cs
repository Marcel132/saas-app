using backend.Api.Controllers;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;

namespace backend.Application.Validators;
public static class QueryParamsValidator
{
  public static Result Validate(QueryParams queryParams, Guid? userId)
  {
    if(userId is not null && userId == Guid.Empty)
      return Result.Failure(new Error(
        DomainCodes.Validation.InvalidValue,
        "Niepoprawne guid",
        HttpResponseState.BadRequest
      ));
    
    if (queryParams.Page <= 0 || queryParams.PageSize <= 0 || queryParams.PageSize > 50)
      return Result.Failure(new Error(
        DomainCodes.Validation.InvalidValue,
        "Niepoprawne parametry",
        HttpResponseState.BadRequest
      ));

    if (!string.IsNullOrWhiteSpace(queryParams.Search) && queryParams.Search.Length > 100)
      return Result.Failure(new Error(
        DomainCodes.Validation.InvalidValue,
        "Niepoprawny parametr 'search'",
        HttpResponseState.BadRequest
      ));

    return Result.Success();
  }
}