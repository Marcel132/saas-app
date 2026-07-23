using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Api.Http;
using backend.Api.Controllers.Auth.DTOs;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Commands;
using backend.Application.Security;
using backend.Application.Services;
using backend.Application.Features.Auth.Dto;

namespace backend.Api.Controllers.Auth.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
  private readonly ICommandHandler<LoginCommand, CredentialsDto> _loginHandler;
  private readonly ICommandHandler<RegisterPentesterCommand, CredentialsDto> _registerPentesterHandler;
  private readonly ICommandHandler<RegisterCompanyCommand, CredentialsDto> _registerCompanyHandler;
  private readonly ICommandHandler<LogoutCommand> _logoutHandler;
  private readonly ICommandHandler<RefreshTokenCommand, CredentialsDto> _refreshTokenHandler;
  public AuthController(
  ICommandHandler<LoginCommand, CredentialsDto> loginHandler,
  ICommandHandler<RegisterPentesterCommand, CredentialsDto> registerPentesterHandler,
  ICommandHandler<RegisterCompanyCommand, CredentialsDto> registerCompanyHandler,
  ICommandHandler<LogoutCommand> logoutHandler,
  ICommandHandler<RefreshTokenCommand, CredentialsDto> refreshTokenHandler
)
  {
    _loginHandler = loginHandler;
    _registerPentesterHandler = registerPentesterHandler;
    _registerCompanyHandler = registerCompanyHandler;
    _logoutHandler = logoutHandler;
    _refreshTokenHandler = refreshTokenHandler;
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
      DomainCodes.Auth.LoginSucceeded
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
      DomainCodes.User.Created
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
    if (credentials.IsFailure)
      return credentials.ToActionResult(
        HttpContext
      );

    var creds = credentials.Value;

    AuthCookies.SetAuthCookie(Response, creds.RefreshToken, creds.AuthToken);

    return credentials.ToActionResult(
      HttpContext,
      "Zarejestrowano",
      DomainCodes.User.Created
    );
  }


  [Authorize]
  [HttpPost("logout")]
  public async Task<IActionResult> LogoutUser(CancellationToken ct)
  {
    var userId = UserContextExtension.GetUserId(User);
    // TODO: Log device info on logout for security auditing
    // TODO: Deleted session and tokens from database on user ip or user agent change to prevent session hijacking

    var result = await _logoutHandler.HandleAsync(new LogoutCommand(userId), ct);

    if (result.IsSuccess)
      AuthCookies.ClearAuthCookie(Response);
    
    return result.ToActionResult(
      HttpContext,
      "Wylogowano",
      DomainCodes.Auth.LogoutSucceeded
    );
  }

  [AllowAnonymous]
  [HttpPost("refresh-token")]
  public async Task<IActionResult> RefreshToken(CancellationToken ct)
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);
    var refreshToken = AuthCookies.GetRefreshToken(Request);

    var command = new RefreshTokenCommand(
      IpAddress: ipAddress,
      UserAgent: userAgent,
      RefreshToken: refreshToken
    );

    var result = await _refreshTokenHandler.HandleAsync(command, ct);
    if (result.IsFailure)
      return result.ToActionResult(
        HttpContext
      );

    var resultValue = result.Value;

    AuthCookies.SetAuthCookie(Response, resultValue.RefreshToken, resultValue.AuthToken);

    return result.ToActionResult(
      HttpContext,
      "Refresh Token updated",
      DomainCodes.Auth.RefreshSucceeded
    );
  }
}