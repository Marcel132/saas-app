using backend.Api.Controllers.Users.DTOs;
using backend.Api.Http;
using backend.Application.Features.Users.Commands;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Entities.Records;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using Moq;
using NUnit.Framework;

namespace backend.Backend.Tests.UnitTests.Application.Features.Users.Commands;

public sealed class UpdateCompanyCommandHandlerTests
{
  [Test]
  public async Task HandleAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
  {
    var command = new UpdateCompanyCommand(
      UserId: Guid.NewGuid(),
      new UpdateCompanyDto()
    );

    var userRepository = new Mock<IUserRepository>();
    var unitOfWork = new Mock<IUnitOfWork>();

    userRepository
      .Setup(x =>
        x.GetByIdAsync(
          command.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((User?)null);

    var handler = new UpdateCompanyCommandHandler(
      userRepository.Object,
      unitOfWork.Object
    );

    var result = await handler.HandleAsync(
      command,
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.User.NotFound));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.NotFound));

    userRepository.Verify(
      x => x.GetByIdAsync(
        command.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    unitOfWork.Verify(
      x => x.SaveChangesAsync(
        It.IsAny<CancellationToken>()
      ), Times.Never
    );
  }

  [Test]
  public async Task HandleAsync_ShouldReturnSuccess_WhenUserExists()
  {
    var user = new User(
      new UserRecord(
        "testtest1@gmail.com123123123",
        "Password123!",
        RoleType.Company
      )
    );
    var profileRecord = new CompanyProfileRecord(
      Nip: "1234567890",
      Name: "No test company",
      Phone: "123456789",
      Country: "Polska",
      City: "Warszawa",
      Street: "Testowa 123",
      PostalCode: "12-345",
      Bio: "Test bio",
      WebsiteUrl: "https://test.com"
    );
    
    user.CreateCompanyProfile(profileRecord);
    
    var command = new UpdateCompanyCommand(
      UserId: user.Id,
      new UpdateCompanyDto
      {
        Name = "Test Company S.A",
        Country = "France"
      }
    );

    var userRepository = new Mock<IUserRepository>();
    var unitOfWork = new Mock<IUnitOfWork>();

    userRepository
      .Setup(x =>
        x.GetByIdAsync(
          user.Id,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(user);


    var handler = new UpdateCompanyCommandHandler(
      userRepository.Object,
      unitOfWork.Object
    );

    var result = await handler.HandleAsync(
      command,
      CancellationToken.None
    );

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(user.CompanyProfile, Is.Not.Null);
    Assert.That(user.CompanyProfile!.Name, Is.EqualTo(command.Dto.Name));
    Assert.That(user.CompanyProfile!.Country, Is.EqualTo(command.Dto.Country));

    userRepository.Verify(
      x => x.GetByIdAsync(
        user.Id,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    unitOfWork.Verify(
      x => x.SaveChangesAsync(
        It.IsAny<CancellationToken>()
      ),
      Times.Once
    );
  }
}