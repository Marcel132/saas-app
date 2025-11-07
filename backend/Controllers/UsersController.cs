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

  public UsersController(UserService userService, TokenService tokenService)
  {
    _userService = userService;
    _tokenService = tokenService;
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
    return Ok(HttpResponseFactory.Ok(HttpContext, users, null, ErrorCodes.Auth.Success));
  }

  [Authorize]
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser()
  {
    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(HttpResponseFactory.Unauthorized<object>(HttpContext, "Invalid or missing user name identifier", ErrorCodes.Auth.InvalidNameIdentifier));
    
    var user = await _userService.GetCurrentUserAsync(userId);
    return Ok(HttpResponseFactory.Ok(HttpContext, user, "Return a user object", ErrorCodes.Auth.Success));
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
      return BadRequest(HttpResponseFactory.BadRequest<object>(HttpContext, "Invalid model state.", ErrorCodes.Validation.MissingRequiredField));

    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(HttpResponseFactory.Unauthorized<object>(HttpContext, "Invalid or missing user name identifier", ErrorCodes.Auth.InvalidNameIdentifier));

    // Attempt to update the user by ID.
    // If the user is not found, return a 404 Not Found response.
    await _userService.UpdateUserAsync(userId, request);
    return Ok(HttpResponseFactory.Ok(HttpContext, "User updated successfully", ErrorCodes.Auth.Success));
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
      return Unauthorized(HttpResponseFactory.Unauthorized<object>(HttpContext, "Invalid or missing user ID claim.", ErrorCodes.Auth.InvalidNameIdentifier));

    // Attempt to delete the user by ID.
    // If the user is not found, return a 404 Not Found response.
    await _userService.DeleteUserAsync(userId);
    return Ok(HttpResponseFactory.Ok(HttpContext, $"User with ID {userId} deleted successfully", ErrorCodes.Auth.Success));
  }

  // path: /users/login
  // This endpoint authenticates a user and generates an auth token.
  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
  {
   if (request == null)
      return BadRequest(HttpResponseFactory.BadRequest<object>(HttpContext, "Request body is required.", ErrorCodes.Validation.MissingRequiredField));

    if (!ModelState.IsValid)
      return BadRequest(HttpResponseFactory.BadRequest<object>(HttpContext, "Invalid model state.", ErrorCodes.Validation.MissingRequiredField));

    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
      return BadRequest(HttpResponseFactory.BadRequest<object>(HttpContext, "Email and Password are required.", ErrorCodes.Validation.MissingRequiredField));


    var user = await _userService.AuthenticateUserAsync(request);

    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
    var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

    var token = await _tokenService.GenerateAuthToken(user.Id, user.Role.ToString(), ip, userAgent);

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

    return Ok(HttpResponseFactory.Ok(HttpContext,"Login successful", ErrorCodes.Auth.Success));
  }

  // path: /users/logout
  // This endpoint logs out a user by revoking their active sessions.
  [Authorize]
  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {

    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(HttpResponseFactory.Unauthorized<object>(HttpContext, "Invalid or missing user ID claim.", ErrorCodes.Auth.InvalidNameIdentifier));

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

      return Ok(HttpResponseFactory.Ok(HttpContext, "Logout successful", ErrorCodes.Auth.Success));
  }

  // Registration Endpoint
  // path: /users/register
  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] UsersModel request)
  {
    // Validate the request model.
    if (!ModelState.IsValid)
      return BadRequest(HttpResponseFactory.BadRequest<object>(HttpContext, "Invalid model state.", ErrorCodes.Validation.MissingRequiredField));

    var user = await _userService.RegisterUserAsync(request);

    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
    var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

    var token = await _tokenService.GenerateAuthToken(user.Id, user.Role.ToString(), ip, userAgent);

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
    
    return Ok(HttpResponseFactory.Ok(HttpContext, new { email = user.Email, id = user.Id, authToken = token.AuthToken }, "User registered successfully.", ErrorCodes.Auth.Success));
  }

  // Generation Endpoint 
  // path: /users/token/generate
  [Authorize]
  [HttpPost("token/auth")]
  public async Task<IActionResult> GenerateToken([FromBody] TokenAuthModel request)
  {
    if (request.UserId <= 0 || string.IsNullOrEmpty(request.Role))
      return BadRequest(new { success = false, message = "UserId and Role are required." });

    if (!ModelState.IsValid)
      return BadRequest(new { success = false, message = "Invalid model state.", details = ModelState });

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

      return Ok(new { success = true, data = new { authToken = token.AuthToken }, message = "Token generated successfully." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
    catch (InvalidOperationException ex)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while generating the token.", details = ex.Message });
    }
  }

  [AllowAnonymous]
  [HttpPost("token/auth/refresh-token")]
  public async Task<IActionResult> RefreshToken()
  {
    if (!Request.Cookies.TryGetValue("RefreshToken", out string? refreshToken))
      return Unauthorized(new { success = false, message = "Refresh token is missing" });

    try
    {
      var token = await _tokenService.RefreshTokenAsync(refreshToken);

      if (token == null)
        return BadRequest("Cannot refresh a token");

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

      return Ok(new { success = true, message = "Token refresh successful" });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (UnauthorizedAccessException ex) { return Unauthorized(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while refreshing the token" });
    }
  }
}