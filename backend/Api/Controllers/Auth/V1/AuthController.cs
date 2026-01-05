using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
  private readonly AuthService _authService;
  public AuthController(
    AuthService authService
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
    // var deviceIp = UserContextExtension.GetUserIp(HttpContext);
    // var userAgent = UserContextExtension.GetUserAgent(HttpContext);

    await _authService.LogoutAsync(userId, Response);

    return NoContent();
  }
}