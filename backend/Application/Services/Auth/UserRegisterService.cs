public class UserRegisterService
{
  private readonly IUserRepository _users;
  private readonly IPasswordHasher _hasher;
  private readonly IRegisterPolicy _policy;
  private readonly UserRoleSynchronizer _roleSynch;
  private readonly IUnitOfWork _unitOfWork;
  public UserRegisterService(
    IUserRepository userRepository,
    IRegisterPolicy registerPolicy,
    IPasswordHasher passwordHasher,
    UserRoleSynchronizer roleSynchronizer,
    IUnitOfWork unitOfWork
  )
  {
    _users = userRepository;
    _policy = registerPolicy;
    _hasher = passwordHasher;
    _roleSynch = roleSynchronizer;
    _unitOfWork = unitOfWork;
  }

  public async Task<User> RegisterAsync(RegisterRequestDto request)
  {
    // TODO: refactor: create emailNormalizes and nicknameNormalized in DB
    // TODO: Move nicknameExists to EnsureCanRegister
    var email = request.Email.Trim().ToLowerInvariant();
    var emailExists = await _users.ExistsByEmailAsync(email);
    var nickname = request.Nickname.Trim().ToLowerInvariant();
    var nicknameExists = await _users.ExistsByNicknameAsync(nickname);
    if(nicknameExists)
      throw new BadRequestAppException("Taka nazwa użytkownika już istnieje");
    _policy.EnsureCanRegister(emailExists, request);

    var passwordHash = _hasher.Hash(request.Password);
    
    var userData = new UserData(request);

    var user = new User(email, passwordHash, request.Role, userData);

    if (request.SpecializationType != null)
    {
      foreach (var spec in request.SpecializationType)
        user.AddSpecialization(spec);
    };

    await _users.AddAsync(user);

    await _roleSynch.SyncAsync(user);
    
    await _unitOfWork.SaveChangesAsync();

    return user;
  }
}