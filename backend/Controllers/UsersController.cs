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


  [HttpGet("{id}")]
  public IActionResult GetUserById(int id)
  {
    // This is a placeholder for the actual user retrieval logic by ID.
    return Ok(new { Message = $"User details for ID: {id}" });
  }

  [HttpPost("/create")]
  public IActionResult CreateUser([FromBody] object user)
  {
    // This is a placeholder for the actual user creation logic.
    return Ok(new { Message = "User created successfully", User = user });
  }

  [HttpPut("/update/{id}")]
  public IActionResult UpdateUser(int id, [FromBody] object user)
  {
    // This is a placeholder for the actual user update logic.
    return Ok(new { Message = $"User with ID: {id} updated successfully", User = user });
  }

  [HttpDelete("/delete/{id}")]
  public IActionResult DeleteUser(int id)
  {
    // This is a placeholder for the actual user deletion logic.
    return Ok(new { Message = $"User with ID: {id} deleted successfully" });
  }
}