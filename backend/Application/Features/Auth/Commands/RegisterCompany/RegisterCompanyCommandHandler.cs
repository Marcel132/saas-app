using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Dto;
using backend.Application.Features.Auth.Shared;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Entities.Records;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Auth.Commands;

public sealed class RegisterCompanyCommandHandler : ICommandHandler<RegisterCompanyCommand, CredentialsDto>
{
  private readonly IUserRepository _userRepo;
  private readonly IPasswordHasher _hasher;
  private readonly IRegisterPolicy _policy;
  private readonly IUnitOfWork _unitOfWork;
  private readonly AuthCredentialsIssuer _issuer;
  private readonly UserRoleSynchronizer _synchronizer;
  public RegisterCompanyCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher hasher,
    IRegisterPolicy registerPolicy,
    IUnitOfWork unitOfWork,
    AuthCredentialsIssuer issuer,
    UserRoleSynchronizer userRoleSynchronizer
  )
  {
    _userRepo = userRepository;
    _hasher = hasher;
    _policy = registerPolicy;
    _unitOfWork = unitOfWork;
    _issuer = issuer;
    _synchronizer = userRoleSynchronizer;
  }
  public async Task<Result<CredentialsDto>> HandleAsync(RegisterCompanyCommand req, CancellationToken ct)
  {
    var emailExists = await _userRepo.ExistsByEmailAsync(req.Email, ct);

    var policyResult = _policy.CanRegisterCompany(emailExists, req);
    if(policyResult.IsFailure)
      return Result<CredentialsDto>.Failure(policyResult.Error);
    
    var passwordHash = _hasher.Hash(req.Password);

    var userRecord = new UserRecord(
      NormalizedEmail: req.Email.Trim().ToLowerInvariant(),
      PasswordHash: passwordHash,
      Role: RoleType.Company
    );
    var profileRecord = new CompanyProfileRecord(
      Nip: req.Nip,
      Name: req.Name,
      Phone: req.Phone,
      Country: req.Country,
      City: req.City,
      Street: req.Street,
      PostalCode: req.PostalCode,
      Bio: req.Bio ?? string.Empty,
      WebsiteUrl: req.WebsiteUrl ?? string.Empty
    );

    var user = new User(userRecord);
    user.CreateCompanyProfile(profileRecord);

    await _userRepo.AddAsync(user, ct);
    await _synchronizer.SyncAsync(user, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    return await _issuer.IssueAsync(user.Id, req.IpAddress, req.UserAgent, ct);
  }
}