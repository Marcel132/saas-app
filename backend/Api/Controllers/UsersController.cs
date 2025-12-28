using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
  private readonly UserService _userService;
  private readonly TokenService _tokenService;
  private readonly AuthCookieService _authCookieService;
  private readonly ILogger _logger;

  public UsersController(
    UserService userService, 
    TokenService tokenService, 
    AuthCookieService authCookieService,
    ILogger<UsersController> logger
  )
  {
    _userService = userService;
    _tokenService = tokenService;
    _authCookieService = authCookieService;
    _logger = logger;
  }

  // -------------------------------
  // path: /users         READ
  // -------------------------------

  // [RequiredRole("Admin")]
  [Authorize]
  [HttpGet]
  public async Task<IActionResult> GetUsers()
  {
    var users = await _userService.GetAllUsersAsync();
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "Users retrieved successfully", 
      HttpStatusCodes.AuthCodes.Success,
      users
      ));
  }

  // -------------------------------
  // path: /users/{id}         READ, UPDATE, DELETE
  // -------------------------------

  [Authorize]
  [HttpGet("{id}")]
  public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
  {
    var user = await _userService.GetUserByIdAsync(userId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      true,
      "User retrieved successfully",
      HttpStatusCodes.AuthCodes.Success,
      user
    ));
  }

  [Authorize]
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateUserById([FromRoute] Guid userId, [FromBody] UpdateUserDto request)
  {
    await _userService.UpdateUserByIdAsync(userId, request);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      true,
      "User updated successfully",
      HttpStatusCodes.AuthCodes.Success
    ));
  }

  [Authorize]
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUserById([FromRoute] Guid userId)
  {
    await _userService.DeleteUserByIdAsync(userId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      true,
      "Deleted user successfully",
      HttpStatusCodes.AuthCodes.Success
    ));
  }

  // -------------------------------
  // path: /users/me         READ, UPDATE, DELETE
  // -------------------------------

  [Authorize]
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser()
  {
    var userId = GetUserClaims.GetUserId(User);
    var user = await _userService.GetCurrentUserAsync(userId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true, 
      "Current user retrieved successfully", 
      HttpStatusCodes.AuthCodes.Success,
      user
      ));
  }

  [Authorize]
  [HttpPut("me")]
  public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateCurrentUserDto request)
  {
    var userId = GetUserClaims.GetUserId(User);
    
    await _userService.UpdateCurrentUserAsync(userId, request);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "User updated successfully", 
      HttpStatusCodes.AuthCodes.Success
    ));
  }
  
  [Authorize]
  [HttpDelete("me")]
  public async Task<IActionResult> DeleteCurrentUser()
  {
    var userId = GetUserClaims.GetUserId(User);
    await _userService.DeleteCurrentUserAsync(userId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      $"User with ID {userId} deleted successfully",
      HttpStatusCodes.AuthCodes.Success
    )); 
  }
}

  // Generation Endpoint 
  // path: /users/token/generate
  // [Authorize]
  // [HttpPost("token/auth")]
  // public async Task<IActionResult> GenerateToken()
  // {
  //   var userIdClaim = GetUserClaims.GetUserId(User);
  //   if (userIdClaim <= 0)
  //     return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
  //       HttpContext, 
  //       HttpResponseState.Unauthorized, 
  //       false,
  //       "You are not allowed to use this method", 
  //       HttpStatusCodes.AuthCodes.InvalidNameIdentifier
  //     ));

  //   var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
  //   var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

  //     var token = await _tokenService.GenerateAuthToken(userIdClaim, ip, userAgent)
  //       ?? throw new ArgumentException("Token generation failed.");

  //     Response.Cookies.Append("AuthToken", token.AuthToken, new CookieOptions
  //     {
  //       HttpOnly = true,
  //       Secure = HttpContext.Request.IsHttps,
  //       SameSite = SameSiteMode.Strict,
  //       Expires = DateTime.UtcNow.AddMinutes(15)
  //     });

  //     // Set the refresh token as an HttpOnly cookie
  //     Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
  //     {
  //       HttpOnly = true,
  //       Secure = HttpContext.Request.IsHttps,
  //       SameSite = SameSiteMode.Strict,
  //       Expires = DateTime.UtcNow.AddDays(7)
  //     });

  //     return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
  //       HttpContext, 
  //       HttpResponseState.Success, 
  //       true,
  //       "Token generated successfully.", 
  //       HttpStatusCodes.AuthCodes.Success, 
  //       new { authToken = token.AuthToken }
  //       ));
  // }

  // [AllowAnonymous]
  // [HttpPost("token/auth/refresh-token")]
  // public async Task<IActionResult> RefreshToken()
  // {
  //   if (!Request.Cookies.TryGetValue("RefreshToken", out string? refreshToken))
  //     return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
  //       HttpContext,
  //       HttpResponseState.Unauthorized,
  //       false,
  //       "Refresh token is missing", 
  //       HttpStatusCodes.AuthCodes.InvalidNameIdentifier
  //     ));


  //   var token = await _tokenService.RefreshTokenAsync(refreshToken);

  //   if (token == null)
  //     return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
  //       HttpContext,
  //       HttpResponseState.BadRequest,
  //       false,
  //       "Cannot refresh a token", 
  //       HttpStatusCodes.AuthCodes.InvalidToken
  //     ));

  //   Response.Cookies.Append("AuthToken", token.AuthToken, new CookieOptions
  //   {
  //     HttpOnly = true,
  //     Secure = HttpContext.Request.IsHttps,
  //     SameSite = SameSiteMode.Strict,
  //     Expires = DateTime.UtcNow.AddMinutes(15)
  //   });

  //   // Set the refresh token as an HttpOnly cookie
  //   Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
  //   {
  //     HttpOnly = true,
  //     Secure = HttpContext.Request.IsHttps,
  //     SameSite = SameSiteMode.Strict,
  //     Expires = DateTime.UtcNow.AddDays(7)
  //   });

  //   return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
  //     HttpContext,
  //     HttpResponseState.Success,
  //     true,
  //     "Token refresh successful",
  //     HttpStatusCodes.AuthCodes.Success
  //   ));
  
  // }
