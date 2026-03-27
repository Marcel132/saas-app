using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
  private readonly AuthService _authService;
  private readonly AuthCookieService _cookieSerivce;
  public AuthController(
    AuthService authService,
    AuthCookieService cookieSerivce
  )
  {
    _authService = authService;
    _cookieSerivce = cookieSerivce;
  }

  // -------------------------------
  // path: /auth
  // -------------------------------

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto request)
  {
    var deviceIp = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    await _authService.LoginAsync(request, deviceIp, userAgent, Response);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "Login successful", 
      DomainErrorCodes.AuthCodes.Success
      ));
  }

  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto request)
  {
    var deviceIp = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    var user = await _authService.RegisterAsync(request, deviceIp, userAgent, Response);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "User registered successfully.", 
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
    // var deviceIp = UserContextExtension.GetUserIp(HttpContext);
    // var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    await _authService.LogoutAsync(userId, Response);

    return NoContent();
  }

  [AllowAnonymous]
  [HttpPost("refresh")]
  public async Task<IActionResult> RefreshToken()
  {
    var deviceIp = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);
    var refreshToken = _cookieSerivce.GetRefreshToken(Request);

    var result = await _authService.RefreshTokenAsync(deviceIp, userAgent, refreshToken);

    _cookieSerivce.SetAuthCookie(Response, result.RefreshToken, result.AuthToken);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "Token refreshed successfully.", 
      DomainErrorCodes.AuthCodes.Success
      ));
  }
}