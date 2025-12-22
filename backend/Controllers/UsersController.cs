using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
  private readonly UserService _userService;
  private readonly TokenService _tokenService;
  private readonly ILogger _logger;

  public UsersController(UserService userService, TokenService tokenService, ILogger<UsersController> logger)
  {
    _userService = userService;
    _tokenService = tokenService;
    _logger = logger;
  }


  // User Management Endpoints
  // path: /users
  [RequiredRole("Admin")]
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
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser()
  {
    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.Unauthorized, 
        false, 
        "Invalid or missing user name identifier", 
        HttpStatusCodes.AuthCodes.InvalidNameIdentifier
        ));
    
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

  // path: /users/{id}
  // This endpoint updates a user by their ID.
  [Authorize]
  [HttpPut]
  public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserModel request)
  {
    // Validate the ID and requested data before proceeding.
    // If the ID is invalid or requested data is null return a 400 Bad Request response. 
    if (!ModelState.IsValid)
      return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.BadRequest, 
        false,
        "Invalid model state.", 
        HttpStatusCodes.ValidationCodes.MissingRequiredField
      ));


    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.Unauthorized, 
        false,
        "Invalid or missing user name identifier", 
        HttpStatusCodes.AuthCodes.InvalidNameIdentifier
      ));
    
    // Attempt to update the user by ID.
    // If the user is not found, return a 404 Not Found response.
    await _userService.UpdateUserAsync(userId, request);
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "User updated successfully", 
      HttpStatusCodes.AuthCodes.Success
    ));
  }


  // path: /users/{id}
  // This endpoint deletes a user by their ID.
  [Authorize]
  [HttpDelete]
  public async Task<IActionResult> DeleteUser()
  {
    //  Validate the ID before proceeding.
    // If the ID is invalid, return a 400 Bad Request response.
    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.Unauthorized, 
        false,
        "Invalid or missing user name identifier", 
        HttpStatusCodes.AuthCodes.InvalidNameIdentifier
      ));

    // Attempt to delete the user by ID.
    // If the user is not found, return a 404 Not Found response.
    await _userService.DeleteUserAsync(userId);
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      $"User with ID {userId} deleted successfully",
      HttpStatusCodes.AuthCodes.Success
    )); 
  }

  // path: /users/login
  // This endpoint authenticates a user and generates an auth token.
  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
  {
    if (!ModelState.IsValid)
      return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.BadRequest, 
        false,
        "Invalid model state.", 
        HttpStatusCodes.ValidationCodes.MissingRequiredField
      ));

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

    var token = await _tokenService.GenerateAuthToken(user.User.Id, user.User.Role.ToString(), ip, userAgent);

    Response.Cookies.Append("AuthToken", token.AuthToken, new CookieOptions
    {
      HttpOnly = true,
      Secure = HttpContext.Request.IsHttps,
      SameSite = SameSiteMode.Strict,
      Expires = DateTime.UtcNow.AddMinutes(15)
    });

    Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
    {
      HttpOnly = true,
      Secure = HttpContext.Request.IsHttps,
      SameSite = SameSiteMode.Strict,
      Expires = DateTime.UtcNow.AddDays(7)
    });

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "Login successful", 
      HttpStatusCodes.AuthCodes.Success
      ));
  }

  // path: /users/logout
  // This endpoint logs out a user by revoking their active sessions.
  [Authorize]
  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {

    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.Unauthorized, 
        false,
        "Invalid or missing user ID claim.", 
        HttpStatusCodes.AuthCodes.InvalidNameIdentifier
        ));

    var deviceIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

      await _userService.LogoutUserAsync(userId, deviceIp);

      // Clear the auth token cookie
      Response.Cookies.Append("AuthToken", "", new CookieOptions
      {
        Expires = DateTime.UtcNow.AddDays(-1),
        HttpOnly = true,
        Secure = HttpContext.Request.IsHttps,
        SameSite = SameSiteMode.Strict
      });
      // Clear the refresh token cookie
      Response.Cookies.Append("RefreshToken", "", new CookieOptions
      {
        Expires = DateTime.UtcNow.AddDays(-1),
        HttpOnly = true,
        Secure = HttpContext.Request.IsHttps,
        SameSite = SameSiteMode.Strict
      });

      return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
        HttpContext, 
        HttpResponseState.Success, 
        true,
        "Logout successful", 
        HttpStatusCodes.AuthCodes.Success
      ));
  }

  // Registration Endpoint
  // path: /users/register
  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
  {
    // Validate the request model.
    if (!ModelState.IsValid)
      return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.BadRequest, 
        false,
        "Invalid model state.", 
        HttpStatusCodes.ValidationCodes.MissingRequiredField
        ));

    var user = await _userService.RegisterUserAsync(request);

    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
    var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

    var token = await _tokenService.GenerateAuthToken(user.User.Id, user.User.Role.ToString(), ip, userAgent);

    Response.Cookies.Append("AuthToken", token.AuthToken, new CookieOptions
    {
      HttpOnly = true,
      Secure = HttpContext.Request.IsHttps,
      SameSite = SameSiteMode.Strict,
      Expires = DateTime.UtcNow.AddMinutes(15)
    });

    Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
    {
      HttpOnly = true,
      Secure = HttpContext.Request.IsHttps,
      SameSite = SameSiteMode.Strict,
      Expires = DateTime.UtcNow.AddDays(7)
    });

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "User registered successfully.", 
      HttpStatusCodes.AuthCodes.Success,  
      new { email = user.User.Email, id = user.User.Id, authToken = token.AuthToken }
      ));
  }

  // Generation Endpoint 
  // path: /users/token/generate
  [Authorize]
  [HttpPost("token/auth")]
  public async Task<IActionResult> GenerateToken([FromBody] TokenAuthModel request)
  {
    if (request.UserId <= 0 || string.IsNullOrEmpty(request.Role))
      return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.BadRequest, 
        false,
        "UserId and Role are required.", 
        HttpStatusCodes.ValidationCodes.MissingRequiredField
        ));

    if (!ModelState.IsValid)
      return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.BadRequest, 
        false,
        "Invalid model state.", 
        HttpStatusCodes.ValidationCodes.MissingRequiredField
        ));

    try
    {
      var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
      var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

      var token = await _tokenService.GenerateAuthToken(request.UserId, request.Role, ip, userAgent)
        ?? throw new ArgumentException("Token generation failed.");

      Response.Cookies.Append("AuthToken", token.AuthToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = HttpContext.Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddMinutes(15)
      });

      // Set the refresh token as an HttpOnly cookie
      Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = HttpContext.Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
      });

      return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
        HttpContext, 
        HttpResponseState.Success, 
        true,
        "Token generated successfully.", 
        HttpStatusCodes.AuthCodes.Success, 
        new { authToken = token.AuthToken }
        ));
    }
    catch (ArgumentException ex) 
    { 
      _logger.LogError(ex, "Error generating token");
      return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.BadRequest, 
        false,
        "Something went wrong while generating the token.", 
        HttpStatusCodes.AuthCodes.InvalidToken
        )); 
    }
    catch (KeyNotFoundException ex) 
    { 
      _logger.LogError(ex, "Key not found during token generation");
      return NotFound(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext,
        HttpResponseState.NotFound, 
        false,
        "Something went wrong while finding the key.", 
        HttpStatusCodes.GeneralCodes.NotFound
        )); 
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogError(ex, "Invalid operation during token generation");
      return Conflict(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext, 
        HttpResponseState.Conflict, 
        false,
        "Conflict occurred while generating the token.", 
        HttpStatusCodes.AuthCodes.InvalidToken
        ));
    }
  }

  [AllowAnonymous]
  [HttpPost("token/auth/refresh-token")]
  public async Task<IActionResult> RefreshToken()
  {
    if (!Request.Cookies.TryGetValue("RefreshToken", out string? refreshToken))
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext,
        HttpResponseState.Unauthorized,
        false,
        "Refresh token is missing", 
        HttpStatusCodes.AuthCodes.InvalidNameIdentifier
        ));

    try
    {
      var token = await _tokenService.RefreshTokenAsync(refreshToken);

      if (token == null)
        return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
          HttpContext,
          HttpResponseState.BadRequest,
          false,
          "Cannot refresh a token", 
          HttpStatusCodes.AuthCodes.InvalidToken
          ));

      Response.Cookies.Append("AuthToken", token.AuthToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = HttpContext.Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddMinutes(15)
      });

      // Set the refresh token as an HttpOnly cookie
      Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = HttpContext.Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
      });

      return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
        HttpContext,
        HttpResponseState.Success,
        true,
        "Token refresh successful",
        HttpStatusCodes.AuthCodes.Success
        ));
    }
    catch (ArgumentException ex) 
    { 
      _logger.LogError(ex, "Error refreshing token");
      return BadRequest(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext,
        HttpResponseState.BadRequest,
        false,
        "Something went wrong while refreshing the token", 
        HttpStatusCodes.AuthCodes.InvalidToken
        )); 
    }
    catch (UnauthorizedAccessException ex) 
    { 
      _logger.LogError(ex, "Unauthorized access during token refresh");
      return Unauthorized(HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext,
        HttpResponseState.Unauthorized,
        false,
        "Unauthorized access while refreshing the token", 
        HttpStatusCodes.AuthCodes.InvalidNameIdentifier
        ));
    }
    catch (System.Exception ex)
    {
      _logger.LogError(ex, "An error occurred while refreshing the token");
      return StatusCode(500, HttpResponseFactory.CreateFailureResponse<object>(
        HttpContext,
        HttpResponseState.ServerError,
        false,
        "An error occurred while refreshing the token", 
        HttpStatusCodes.GeneralCodes.ServerError
        ));
    }
  }
}