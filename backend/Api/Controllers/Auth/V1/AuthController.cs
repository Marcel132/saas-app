using backend.Api.Controllers.Auth.DTOs;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Commands;
using backend.Application.Security;
using backend.Application.Services;
using backend.Application.Features.Auth.Dto;
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
  private readonly ICommandHandler<RegisterPentesterCommand, CredentialsDto> _registerPentesterHandler;
  private readonly ICommandHandler<RegisterCompanyCommand, CredentialsDto> _registerCompanyHandler;
  public AuthController(
  IAuthService authService,
  ICommandHandler<LoginCommand, CredentialsDto> loginHandler,
  ICommandHandler<RegisterPentesterCommand, CredentialsDto> registerPentesterHandler,
  ICommandHandler<RegisterCompanyCommand, CredentialsDto> registerCompanyHandler
)
  {
    _authService = authService;
    _loginHandler = loginHandler;
    _registerPentesterHandler = registerPentesterHandler;
    _registerCompanyHandler = registerCompanyHandler;
  }

  // -------------------------------
  // path: /auth
  // -------------------------------

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto req, CancellationToken ct)
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    var command = new LoginCommand(
      req.Email,
      req.Password,
      ipAddress,
      userAgent
    );

    var credentials = await _loginHandler.HandleAsync(command, ct);

    if (credentials.IsFailure)
      return credentials.ToActionResult(
        HttpContext
      );

    AuthCookies.SetAuthCookie(Response, credentials.Value.RefreshToken, credentials.Value.AuthToken);

    return credentials.ToActionResult(
      HttpContext,
      "Zalogowano",
      DomainErrorCodes.AuthCodes.Success
    );

  }

  [AllowAnonymous]
  [HttpPost("register/pentester")]
  public async Task<IActionResult> RegisterPentester([FromBody] RegisterPentesterRequestDto req, CancellationToken ct)
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    var command = new RegisterPentesterCommand(
      Email: req.Email,
      Password: req.Password,
      FirstName: req.FirstName,
      LastName: req.LastName,
      NickName: req.NickName,
      Phone: req.Phone,
      Country: req.Country,
      City: req.City,
      Street: req.Street,
      PostalCode: req.PostalCode,
      Bio: req.Bio,
      GithubUrl: req.GithubUrl,
      LinkedinUrl: req.LinkedinUrl,
      ExperienceLevel: req.ExperienceLevel,
      IpAddress: ipAddress,
      UserAgent: userAgent
    );

    var credentials = await _registerPentesterHandler.HandleAsync(command, ct);
    if (credentials.IsFailure)
      return credentials.ToActionResult(
        HttpContext
      );

    var creds = credentials.Value;

    AuthCookies.SetAuthCookie(Response, creds.RefreshToken, creds.AuthToken);

    return credentials.ToActionResult(
      HttpContext,
      "Zarejestrowano",
      DomainErrorCodes.AuthCodes.Success
    );
  }

  [AllowAnonymous]
  [HttpPost("register/company")]
  public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyRequestDto req, CancellationToken ct)
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    var command = new RegisterCompanyCommand(
      Email: req.Email,
      Password: req.Password,
      Nip: req.Nip,
      Name: req.Name,
      Phone: req.Phone,
      City: req.City,
      Country: req.Country,
      PostalCode: req.PostalCode,
      Street: req.Street,
      Bio: req.Bio ?? string.Empty,
      WebsiteUrl: req.WebsiteUrl ?? string.Empty,
      IpAddress: ipAddress,
      UserAgent: userAgent
    );

    var credentials = await _registerCompanyHandler.HandleAsync(command, ct);
    if(credentials.IsFailure)
      return credentials.ToActionResult(
        HttpContext
      );

    var creds = credentials.Value;

    AuthCookies.SetAuthCookie(Response, creds.RefreshToken, creds.AuthToken);

    return credentials.ToActionResult(
      HttpContext,
      "Zarejestrowano",
      DomainErrorCodes.AuthCodes.Success
    );
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