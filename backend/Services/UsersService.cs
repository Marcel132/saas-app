using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserService
{
  private readonly AppDbContext _context;
  public UserService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<UsersModel>> GetAllUsersAsync()
  {
    var users = await _context.users
    .Include(u => u.Session)
    .Include(u => u.UserData)
    .Include(u => u.Opinions)
    .Include(u => u.ApiLogs)
    .ToListAsync();

    return users;
  }

  public async Task<UsersModel> GetUserByIdAsync(int userId)
  {
    var user = await _context.users
      .Include(u => u.Session)
      .Include(u => u.UserData)
      .Include(u => u.Opinions)
      .Include(u => u.ApiLogs)
      .FirstOrDefaultAsync(u => u.Id == userId);


    if (user == null)
    {
      throw new KeyNotFoundException($"User with ID {userId} not found.");
    }
    else
    {
      return user;
    }
  }

  public async Task<bool> UpdateUserAsync(int userId, UpdateUserModel userModel)
  {
    if (userId <= 0 || userModel == null)
    {
      throw new ArgumentException("Invalid user ID or update data.");
    }

    try
    {
      var user = await _context.users.FindAsync(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
      var userData = await _context.user_data.FirstOrDefaultAsync(ud => ud.UserId == userId) ?? throw new KeyNotFoundException($"User data for user with ID {userId} not found.");

      user.PasswordHash = string.IsNullOrWhiteSpace(userModel.PasswordHash) ? user.PasswordHash : userModel.PasswordHash;
      if (Enum.IsDefined(typeof(RoleType), userModel.Role))
        user.Role = userModel.Role;
      user.IsActive = userModel.IsActive;

      userData.FirstName = string.IsNullOrWhiteSpace(userModel.FirstName) ? userData.FirstName : userModel.FirstName;
      userData.LastName = string.IsNullOrWhiteSpace(userModel.LastName) ? userData.LastName : userModel.LastName;
      userData.PhoneNumber = string.IsNullOrWhiteSpace(userModel.PhoneNumber) ? userData.PhoneNumber : userModel.PhoneNumber;
      userData.City = string.IsNullOrWhiteSpace(userModel.City) ? userData.City : userModel.City;
      userData.Country = string.IsNullOrWhiteSpace(userModel.Country) ? userData.Country : userModel.Country;
      userData.PostalCode = string.IsNullOrWhiteSpace(userModel.PostalCode) ? userData.PostalCode : userModel.PostalCode;
      userData.Street = string.IsNullOrWhiteSpace(userModel.Street) ? userData.Street : userModel.Street;
      userData.CompanyName = string.IsNullOrWhiteSpace(userModel.CompanyName) ? userData.CompanyName : userModel.CompanyName;
      userData.CompanyNip = string.IsNullOrWhiteSpace(userModel.CompanyNip) ? userData.CompanyNip : userModel.CompanyNip;

      userData.IsEmailVerified = userModel.IsEmailVerified;
      userData.IsTwoFactorEnabled = userModel.IsTwoFactorEnabled;
      userData.IsProfileCompleted = userModel.IsProfileCompleted;
      
      await _context.SaveChangesAsync();

      return true;
    }
    catch (Exception ex)
    {
      throw new Exception(new { Message = "An error occurred while updating the user.", Details = ex.Message }.ToString());
    }
  }

  public async Task<IActionResult> DeleteUserAsync(int userId)
  {
    try
    {
      var user = await _context.users.FindAsync(userId);
      if (user == null)
      {
        return new NotFoundObjectResult(new { Message = $"User with ID {userId} not found." });
      }

      _context.users.Remove(user);

      var userSession = await _context.sessions.FirstOrDefaultAsync(s => s.UserId == userId);
      if (userSession != null)
      {
        _context.sessions.Remove(userSession);
      }

      var userData = await _context.user_data.FirstOrDefaultAsync(ud => ud.UserId == userId);
      if (userData != null)
      {
        _context.user_data.Remove(userData);
      }

      var userOpinions = await _context.opinions.Where(o => o.TargetId == userId).ToListAsync();
      if (userOpinions.Any())
      {
        _context.opinions.RemoveRange(userOpinions);
      }

      var userApiLogs = await _context.api_logs.Where(al => al.UserId == userId).ToListAsync();
      if (userApiLogs.Any())
      {
        _context.api_logs.RemoveRange(userApiLogs);
      }

      await _context.SaveChangesAsync();

      return new OkObjectResult($"User with ID {userId} deleted successfully.");
    }
    catch (Exception ex)
    {
      return new ObjectResult(new { Message = "An error occurred while deleting the user.", Details = ex.Message }) { StatusCode = 500 };
    }
  }
  public async Task<UsersModel> RegisterUserInTableUsersAsync(UsersModel user)
  {
    if (user == null)
    {
      throw new ArgumentNullException("User data is null.");
    }
    else if (user.Email == null || user.PasswordHash == null)
    {
      throw new ArgumentNullException("Email and PasswordHash are required.");
    }

    try
    {
      // Check if the user already exists
      var existingUser = await _context.users.FirstOrDefaultAsync(u => u.Email == user.Email);
      if (existingUser != null)
      {
        throw new ArgumentException("User with this email already exists.");
      }

      // Add the new user to the database
      _context.users.Add(user);
      await _context.SaveChangesAsync();

      return user;
    }
    catch (Exception ex)
    {
      throw new Exception(new { Message = "An error occurred while registering the user.", Details = ex.Message }.ToString());
    }
  }
  
  public async Task<IActionResult> RegisterUserDataInTableUserDataAsync(UserDataModel userData)
  {
    if (userData == null)
    {
      return new BadRequestObjectResult("User data is null.");
    }
    else if (userData.UserId <= 0)
    {
      return new BadRequestObjectResult("UserId is required.");
    }
    else if (string.IsNullOrEmpty(userData.FirstName) || string.IsNullOrEmpty(userData.LastName))
    {
      return new BadRequestObjectResult("FirstName and LastName are required.");
    }
    else if (string.IsNullOrEmpty(userData.PhoneNumber) || string.IsNullOrEmpty(userData.Country) ||
             string.IsNullOrEmpty(userData.PostalCode) || string.IsNullOrEmpty(userData.Street))
    {
      return new BadRequestObjectResult("PhoneNumber, Country, PostalCode, and Street are required.");
    }

    try
    {
      // Check if the user data already exists
      var existingUserData = await _context.user_data.FirstOrDefaultAsync(ud => ud.UserId == userData.UserId);
      if (existingUserData != null)
      {
        return new ConflictObjectResult("User data for this user already exists.");
      }

      // Add the new user data to the database
      _context.user_data.Add(userData);
      await _context.SaveChangesAsync();

      return new OkObjectResult(userData);
    }
    catch (Exception ex)
    {
      return new ObjectResult(new { Message = "An error occurred while registering the user.", Details = ex.Message }) { StatusCode = 500 };
    }
  }
}
