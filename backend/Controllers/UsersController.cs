using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

  [Authorize(Roles = "Admin")]
  [HttpGet]
  public async Task<IActionResult> GetUsers()
  {
    // This is a placeholder for the actual user retrieval logic.
    var users = await _userService.GetAllUsersAsync();
    return Ok(users);
  }


  [Authorize(Roles = "Admin")]
  [HttpGet("{id}")]
  public async Task<IActionResult> GetUserById(int id)
  {
    if (id <= 0)
    {
      return BadRequest(new { Message = "Invalid user ID." });
    }
    // This is a placeholder for the actual user retrieval logic by ID.
    try
    {
      var user = await _userService.GetUserByIdAsync(id);
      return Ok(user);
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { Message = ex.Message });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { Message = "An error occurred while retrieving the user.", Details = ex.Message });
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserModel updateUser)
  {
    // This is a placeholder for the actual user update logic.
    if (id <= 0 || updateUser == null)
    {
      return BadRequest(new { Message = "Invalid user ID or update data." });
    }

    try
    {
      var updatedUser = await _userService.UpdateUserAsync(id, updateUser);
      if (updatedUser == false)
      {
        return NotFound(new { Message = "User not found." });
      }

      return Ok(new { Message = "User updated successfully", User = updatedUser });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { Message = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { Message = ex.Message });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { Message = "An error occurred while updating the user.", Details = ex.Message });
    }

  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(int id)
  {
    if (id <= 0)
    {
      return BadRequest(new { Message = "Invalid user ID." });
    }
    
    try
    {
      await _userService.DeleteUserAsync(id);
      return Ok(new { Message = "User deleted successfully." });
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { Message = ex.Message });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { Message = ex.Message });
    }
    catch (System.Exception)
    {
      return StatusCode(500, new { Message = "An error occurred while deleting the user." });
    }
  }


  [HttpPost("login")]
  public IActionResult Login([FromBody] object loginDetails)
  {
    // This is a placeholder for the actual login logic.
    return Ok(new { Message = "Login successful", Details = loginDetails });
  }

  [HttpPost("logout")]
  public IActionResult Logout([FromBody] object logoutDetails)
  {
    // This is a placeholder for the actual logout logic.
    return Ok(new { Message = "Logout successful", Details = logoutDetails });
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
  {

    if (request == null)
    {
      return BadRequest(new { Message = "Request cannot be null." });
    }

    try
    {
      var user = await _userService.RegisterModelAsync(request);
      if (user == null)
      {
        return BadRequest(new { Message = "Failed to register user." });
      }

      return Ok(new { Message = "User registered successfully", UserEmail = user.Email, UserId = user.Id });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { Message = ex.Message });
    }

    catch (System.Exception)
    {
      return StatusCode(500, new { Message = "An error occurred while registering the user." });
    }
  }


  // Token

  [HttpPost("token/generate")]
  public async Task<IActionResult> GenerateToken([FromBody] TokenAuthModel request)
  {
    if (request.UserId <= 0 || string.IsNullOrEmpty(request.Role))
    {
      return BadRequest(new { Message = "UserId and Role are required." });
    }

    try
    {
      var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
      var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

      var token = await _tokenService.GenerateToken(request.UserId, request.Role, ip, userAgent);
      return Ok(new { Token = token });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { Message = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { Message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
      return StatusCode(500, new { Message = "An error occurred while generating the token.", Details = ex.Message });
    }
  }
}