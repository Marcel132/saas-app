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
  // path: /users         READ, UPDATE, DELETE
  // -------------------------------

  // [RequiredRole("Admin")]
  [Authorize]
  [HttpGet]
  public async Task<IActionResult> GetUsers()
  {
    // This endpoint retrieves all users.
    // It includes their session, user data, opinions, and API logs.
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

  [Authorize]
  [HttpPut]
  public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateCurrentUserDto request)
  {
    var userId = GetUserClaims.GetUserId(User);
    
    // Attempt to update the user by ID.
    // If the user is not found, return a 404 Not Found response.
    await _userService.UpdateCurrentUserAsync(userId, request);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "User updated successfully", 
      HttpStatusCodes.AuthCodes.Success
    ));
  }
  

  // -------------------------------
  // path: /users/{id}         READ
  // -------------------------------

  [Authorize]
  [HttpGet("{id}")]
  public async Task<IActionResult> GetUserById([FromRoute] Guid id)
  {
    return NotFound(HttpResponseFactory.CreateFailureResponse<object>(
      HttpContext, 
      HttpResponseState.NotFound, 
      false,
      $"User with ID {id} not found",
      HttpStatusCodes.GeneralCodes.NotFound
      ));
  }

  // -------------------------------
  // path: /users/me         READ
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
  [HttpDelete("me")]
  public async Task<IActionResult> DeleteCurrentUser()
  {
    //  Validate the ID before proceeding.
    // If the ID is invalid, return a 400 Bad Request response.
    var userId = GetUserClaims.GetUserId(User);

    // Attempt to delete the user by ID.
    // If the user is not found, return a 404 Not Found response.
    await _userService.DeleteCurrentUserAsync(userId);
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      $"User with ID {userId} deleted successfully",
      HttpStatusCodes.AuthCodes.Success
    )); 
  }
  
  // -------------------------------
  // path: /users/login         CREATE
  // -------------------------------

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
  {
    var user = await _userService.AuthenticateUserAsync(request);
    
    if (!user.Success)
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.Unauthorized, 
        false,
        "Authentication failed.", 
        user.HttpCode
        ));

    if(user.User == null)
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.Unauthorized, 
        false,
        "Authentication failed.", 
        user.HttpCode
        ));

    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
    var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

    var token = await _tokenService.GenerateAuthToken(user.User.Id, ip, userAgent);

    _authCookieService.SetAuthCookie(Response, token);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "Login successful", 
      HttpStatusCodes.AuthCodes.Success
      ));
  }

  // -------------------------------
  // path: /users/logout         UPDATE
  // -------------------------------

  [Authorize]
  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    var userId = GetUserClaims.GetUserId(User);

    var deviceIp = HttpContext.Connection.RemoteIpAddress?.ToString();
    if(string.IsNullOrEmpty(deviceIp))
      return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.BadRequest, 
        false,
        "Unable to determine device IP address.", 
        HttpStatusCodes.AuthCodes.BadRequest
      ));
    
    await _userService.LogoutUserAsync(userId, deviceIp);
    _authCookieService.ClearAuthCookie(Response);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "Logout successful", 
      HttpStatusCodes.AuthCodes.Success,
      $"UserId: {userId} logged out from IP: {deviceIp}"
    ));
  }

  // -------------------------------
  // path: /users/register         CREATE
  // -------------------------------

  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
  {
    _logger.LogInformation("Registering user with email: {Email}", request.Email);

    var user = await _userService.RegisterUserAsync(request);
    if (user.Id <= Guid.Empty)
      return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.BadRequest, 
        false,
        "Failed to register user.", 
        HttpStatusCodes.AuthCodes.InvalidCredentials
      ));

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
}