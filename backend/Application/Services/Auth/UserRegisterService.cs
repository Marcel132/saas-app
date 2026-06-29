using backend.Api.Controllers.Auth.DTOs;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Entities.Records;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Services;

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

  public async Task<User> RegisterPentesterAsync(RegisterPentesterRequestDto req)
  {
    (req.Email, req.NickName) = NormalizeData(req.Email, req.NickName);

    var emailExists = await _users.ExistsByEmailAsync(req.Email);
    var nicknameExists = await _users.ExistsByNicknameAsync(req.NickName);

    _policy.EnsureCanRegisterPentester(emailExists, nicknameExists, req);

    var passwordHash = _hasher.Hash(req.Password);


    var userRecord = new UserRecord(req.Email, passwordHash, RoleType.Pentester);
    var profile= new PentesterProfileRecord(
      FirstName: req.FirstName,
      LastName: req.LastName,
      NickName: req.NickName,
      Phone: req.Phone,
      Country: req.Country,
      City: req.City,
      Street: req.Street,
      PostalCode: req.PostalCode,
      Bio: req.Bio,
      GithubUrl: req.GithubUrl ?? string.Empty,
      LinkedinUrl: req.LinkedinUrl ?? string.Empty,
      Certificates: [],
      Experience: ExperienceLevel.None
    );

    using var transaction = await _unitOfWork.BeginTransactionAsync();
    
    var user = new User(userRecord);
    user.CreatePentesterProfile(profile);

    await _users.AddAsync(user);
    await _roleSynch.SyncAsync(user);

    await _unitOfWork.SaveChangesAsync();

    await transaction.CommitAsync();

    return user;

  }
  public async Task<User> RegisterCompanyAsync(RegisterCompanyRequestDto req)
  {
    req.Email = req.Email.Trim().ToLowerInvariant();

    var emailExists = await _users.ExistsByEmailAsync(req.Email);

    _policy.EnsureCanRegisterCompany(emailExists, req);

    var passwordHash = _hasher.Hash(req.Password);

    var userRecord = new UserRecord(req.Email, passwordHash, RoleType.Company);
    var profile = new CompanyProfileRecord(
      req.Nip,
      req.Name,
      req.Phone,
      req.Country,
      req.City,
      req.Street,
      req.PostalCode,
      req.Bio,
      req.WebsiteUrl
    );
    using var transaction = await _unitOfWork.BeginTransactionAsync();

    var user = new User(userRecord);
    user.CreateCompanyProfile(profile);

    await _users.AddAsync(user);
    await _roleSynch.SyncAsync(user);
    await _unitOfWork.SaveChangesAsync();

    await transaction.CommitAsync();

    return user;
  }

  private static (string email, string nickname) NormalizeData(string email, string nickname)
  {
    var normalizedEmail = email.Trim().ToLowerInvariant();
    var normalizedNickname = nickname.Trim();

    return (normalizedEmail, normalizedNickname);
  }
}