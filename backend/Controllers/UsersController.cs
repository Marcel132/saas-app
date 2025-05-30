using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
  private readonly UserService _userService;

  public UsersController(UserService userService)
  {
    _userService = userService;
  }
  // [Authorize(Roles = "Admin")]
  [HttpGet]
  public IActionResult GetUsers()
  {
    // This is a placeholder for the actual user retrieval logic.
    var users = _userService.GetAllUsersAsync().Result;
    return Ok(users);
  }


  // [Authorize(Roles = "Admin")]
  [HttpGet("{id}")]
  public IActionResult GetUserById(int id)
  {
    // This is a placeholder for the actual user retrieval logic by ID.
    var user = _userService.GetUserByIdAsync(id).Result;

    try
    {
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

  [HttpPut("update/{id}")]
  public IActionResult UpdateUser(int id, [FromBody] object user)
  {
    // This is a placeholder for the actual user update logic.
    return Ok(new { Message = $"User with ID: {id} updated successfully", User = user });
  }

  [HttpDelete("delete/{id}")]
  public IActionResult DeleteUser(int id)
  {
    // This is a placeholder for the actual user deletion logic.
    return Ok(new { Message = $"User with ID: {id} deleted successfully" });
  }


  [HttpPost("account/login")]
  public IActionResult Login([FromBody] object loginDetails)
  {
    // This is a placeholder for the actual login logic.
    return Ok(new { Message = "Login successful", Details = loginDetails });
  }

  [HttpPost("account/logout")]
  public IActionResult Logout([FromBody] object logoutDetails)
  {
    // This is a placeholder for the actual logout logic.
    return Ok(new { Message = "Logout successful", Details = logoutDetails });
  }

  [HttpPost("account/new/register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
  {
    // This is a placeholder for the actual registration logic.
    if (request.User == null || request.UserData == null)
    {
      return BadRequest(new { Message = "Brak wszystkich danych" });
    }

    try
    {
      var user = await _userService.RegisterUserInTableUsersAsync(request.User);
      if (user == null)
      {
        return BadRequest(new { Message = "Nie udało się zarejestrować użytkownika" });
      }
      request.UserData.UserId = user.Id;
      var userData = await _userService.RegisterUserDataInTableUserDataAsync(request.UserData);

      if (user == null || userData == null)
      {
        return BadRequest(new { Message = "Nie udało się zarejestrować użytkownika" });
      }
      else {
        return Ok(new { Message = "Użytkownik zarejestrowany pomyślnie", User = user, UserData = userData });
      }
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { Message = "An error occurred while registering the user.", Details = ex.Message });
    }

  }
}