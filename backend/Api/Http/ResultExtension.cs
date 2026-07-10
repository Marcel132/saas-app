namespace backend.Api.Http;

using backend.Application.Abstractions.CQRS;
using Microsoft.AspNetCore.Mvc;

public static class ResultExtensions
{
  public static IActionResult ToActionResult<T>(
    this Result<T> result,
    HttpContext context,
    string? successMessage = null,
    string? successCode = null)
  {
    if (result.IsFailure)
    {
      var failureBody = HttpResponseFactory.CreateFailureResponse<object>(
        context, result.Error.State, result.Error.Message, result.Error.Code);

      return new ObjectResult(failureBody) { StatusCode = (int)result.Error.State };
    }

    var successBody = HttpResponseFactory.CreateSuccessResponse(
      context, HttpResponseState.Success, successMessage, successCode, result.Value);

    return new ObjectResult(successBody) { StatusCode = (int)HttpResponseState.Success };
  }

  public static IActionResult ToActionResult(
    this Result result,
    HttpContext context,
    string? successMessage = null,
    string? successCode = null)
  {
    if (result.IsFailure)
    {
      var failureBody = HttpResponseFactory.CreateFailureResponse<object>(
        context, result.Error.State, result.Error.Message, result.Error.Code);

      return new ObjectResult(failureBody) { StatusCode = (int)result.Error.State };
    }

    var successBody = HttpResponseFactory.CreateSuccessResponse<object>(
      context, HttpResponseState.Success, successMessage, successCode);

    return new ObjectResult(successBody) { StatusCode = (int)HttpResponseState.Success };
  }
}