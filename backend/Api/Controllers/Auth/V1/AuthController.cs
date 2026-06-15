using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;
  public AuthController(
    IAuthService authService
  )
  {
    _authService = authService;
  }

  // -------------------------------
  // path: /auth
  // -------------------------------

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto request)
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    var credentials = await _authService.LoginAsync(request, ipAddress, userAgent);

    AuthCookies.SetAuthCookie(Response, credentials.RefreshToken, credentials.AuthToken);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Zalogowano", 
      DomainErrorCodes.AuthCodes.Success
      ));
  }

  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto request)
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    var credentials = await _authService.RegisterAsync(request, ipAddress, userAgent);

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