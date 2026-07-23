using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Users.Commands;

public sealed class UpdateCompanyCommandHandler : ICommandHandler<UpdateCompanyCommand>
{
  private readonly IUserRepository _repo;
  private readonly IUnitOfWork _unitOfWork;
  public UpdateCompanyCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
  )
  {
    _repo = userRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> HandleAsync(UpdateCompanyCommand command, CancellationToken ct)
  {
    var user = await _repo.GetByIdAsync(command.UserId, ct);

    if( user is null)
      return Result.Failure(new Error(
        DomainCodes.User.NotFound,
        "Nie znaleziono użytkownika",
        HttpResponseState.NotFound
      ));

    var dto = command.Dto;
    user.UpdateCompanyProfile(
      name: dto.Name,
      phone: dto.Phone,
      country: dto.Country,
      city: dto.City,
      street: dto.Street,
      postalCode: dto.PostalCode,
      bio: dto.Bio,
      websiteUrl: dto.WebsiteUrl
    );

    await _unitOfWork.SaveChangesAsync(ct);

    return Result.Success();
  }
}