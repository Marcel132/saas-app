using backend.Api.Controllers.Auth.DTOs;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Commands;
using backend.Application.Security;
using backend.Application.Services;
using backend.Application.Services.Auth.DTOs;
using backend.Domain.Interfaces.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers.Auth.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;
  private readonly ICommandHandler<LoginCommand, CredentialsDto> _loginHandler;
  public AuthController(
  IAuthService authService,
  ICommandHandler<LoginCommand, CredentialsDto> loginHandler
)
  {
    _authService = authService;
    _loginHandler = loginHandler;
  }

  // -------------------------------
  // path: /auth
  // -------------------------------

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto req)
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    var command = new LoginCommand(
      req.Email, req.Password, ipAddress, userAgent
    );

    var credentials = await _loginHandler.HandleAsync(command);

    if (credentials.IsFailure)
      if (credentials.IsFailure)
        return credentials.ToActionResult(HttpContext);

    AuthCookies.SetAuthCookie(Response, credentials.Value.RefreshToken, credentials.Value.AuthToken);

    return credentials.ToActionResult(HttpContext, "Zalogowano", DomainErrorCodes.AuthCodes.Success);

  }

  [AllowAnonymous]
  [HttpPost("register/pentester")]
  public async Task<IActionResult> RegisterUser([FromBody] RegisterPentesterRequestDto req)
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    var credentials = await _authService.RegisterPentesterAsync(req, ipAddress, userAgent);

    AuthCookies.SetAuthCookie(Response, credentials.RefreshToken, credentials.AuthToken);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Zarejestrowano",
      DomainErrorCodes.AuthCodes.Success
      ));
  }

  [AllowAnonymous]
  [HttpPost("register/company")]
  public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyRequestDto req)
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    var credentials = await _authService.RegisterCompanyAsync(req, ipAddress, userAgent);

    AuthCookies.SetAuthCookie(Response, credentials.RefreshToken, credentials.AuthToken);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Zarejestrowano",
      DomainErrorCodes.AuthCodes.Success
      ));
  }


  [Authorize]
  [HttpPost("logout")]
  public async Task<IActionResult> LogoutUser()
  {
    var userId = UserContextExtension.GetUserId(User);
    // TODO: Log device info on logout for security auditing
    // TODO: Deleted session and tokens from database on user ip or user agent change to prevent session hijacking

    await _authService.LogoutAsync(userId);
    AuthCookies.ClearAuthCookie(Response);

    return NoContent();
  }

  [AllowAnonymous]
  [HttpPost("refresh-token")]
  public async Task<IActionResult> RefreshToken()
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);
    var refreshToken = AuthCookies.GetRefreshToken(Request);

    var result = await _authService.RefreshTokenAsync(ipAddress, userAgent, refreshToken);

    AuthCookies.SetAuthCookie(Response, result.RefreshToken, result.AuthToken);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "Odświeżono token",
      DomainErrorCodes.AuthCodes.Success
      ));
  }
}