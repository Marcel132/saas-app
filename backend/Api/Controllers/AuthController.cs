using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
  private readonly AuthService _authService;
  private readonly TokenService _tokenService;
  private readonly AuthCookieService _authCookieService;
  private readonly ILogger _logger;
  public AuthController(
    AuthService authService,
    TokenService tokenService, 
    AuthCookieService authCookieService,
    ILogger<AuthController> logger
  )
  {
    _authService = authService;
    _tokenService = tokenService;
    _authCookieService = authCookieService;
    _logger = logger;   
  }

  // -------------------------------
  // path: /auth
  // -------------------------------

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto request)
  {
    var user = await _authService.AuthenticateUserAsync(request);
    if (!user.Success)
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.Unauthorized, 
        false,
        "Authentication failed.", 
        user.HttpCode
        ));

    if(user.userId == Guid.Empty)
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.Unauthorized, 
        false,
        "Authentication failed.", 
        user.HttpCode
        ));

    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
    var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

    var token = await _tokenService.GenerateAuthToken(user.userId, ip, userAgent);

    _authCookieService.SetAuthCookie(Response, token);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "Login successful", 
      HttpStatusCodes.AuthCodes.Success
      ));
  }

  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto request)
  {
    var user = await _authService.RegisterUserAsync(request);

    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
    var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

    var token = await _tokenService.GenerateAuthToken(user.Id, ip, userAgent);

    _authCookieService.SetAuthCookie(Response, token);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "User registered successfully.", 
      HttpStatusCodes.AuthCodes.Success,  
      new { id = user.Id}
      ));
  }

  [Authorize]
  [HttpPost("logout")]
  public async Task<IActionResult> LogoutUser()
  {
    var userId = GetUserClaims.GetUserId(User);
    var deviceIp = GetUserClaims.GetUserIp(HttpContext);

    await _authService.LogoutUserAsync(userId, deviceIp);
    _authCookieService.ClearAuthCookie(Response);

    return NoContent();
  }
}