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


    // var token = await _tokenService.GenerateAuthToken(user.Id, ip, userAgent, user.Permissions);

    // _authCookieService.SetAuthCookie(Response, token);

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
    var deviceIp = UserContextExtension.GetUserIp(HttpContext);
    var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    await _authService.LogoutAsync(userId, Response);

    return NoContent();
  }
}