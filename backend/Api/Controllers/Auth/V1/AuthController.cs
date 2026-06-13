using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;
  private readonly AuthCookieService _cookieService;
  public AuthController(
    IAuthService authService,
    AuthCookieService cookieService
  )
  {
    _authService = authService;
    _cookieService = cookieService;
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

    await _authService.LoginAsync(request, ipAddress, userAgent, Response);

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

    var user = await _authService.RegisterAsync(request, ipAddress, userAgent, Response);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Zarejestrowano", 
      DomainErrorCodes.AuthCodes.Success,  
      new { id = user.userId}
      ));
  }

  [Authorize]
  [HttpPost("logout")]
  public async Task<IActionResult> LogoutUser()
  {
    var userId = UserContextExtension.GetUserId(User);
    // TODO: Log device info on logout for security auditing
    // TODO: Deleted session and tokens from database on user ip or user agent change to prevent session hijacking

    await _authService.LogoutAsync(userId, Response);

    return NoContent();
  }

  [AllowAnonymous]
  [HttpPost("refresh-token")]
  public async Task<IActionResult> RefreshToken()
  {
    var ipAddress = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);
    var refreshToken = _cookieService.GetRefreshToken(Request);

    var result = await _authService.RefreshTokenAsync(ipAddress, userAgent, refreshToken);

    _cookieService.SetAuthCookie(Response, result.RefreshToken, result.AuthToken);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Odświeżono token", 
      DomainErrorCodes.AuthCodes.Success
      ));
  }
}