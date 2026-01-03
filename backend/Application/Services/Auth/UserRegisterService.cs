using NUnit.Framework;

public class UserRegisterService
{
  private readonly IUserRepository _users;
  private readonly IPasswordHasher _hasher;
  private readonly IRegisterPolicy _policy;
  public UserRegisterService(
    IUserRepository userRepository,
    IRegisterPolicy registerPolicy,
    IPasswordHasher passwordHasher
  )
  {
    _users = userRepository;
    _policy = registerPolicy;
    _hasher = passwordHasher;
  }

  public async Task<User> RegisterAsync(RegisterRequestDto request)
  {
    var email = request.Email.Trim().ToLowerInvariant();
    var isExists = await _users.ExistsByEmailAsync(email);
    _policy.EnsureCanRegister(isExists, request);

    var passwordHash = _hasher.Hash(request.Password);

    var user = new User(email, passwordHash);

    if (request.SpecializationType != null)
    {
      foreach (var spec in request.SpecializationType)
        user.AddSpecialization(spec);
    }

    user.UpdateUserData(        
      request.FirstName,
      request.LastName,
      request.PhoneNumber,
      request.Skills,
      request.City,
      request.Country,
      request.PostalCode,
      request.Street,
      request.CompanyName,
      request.CompanyNip 
    );

    await _users.AddAsync(user);
    return user;
  }
}