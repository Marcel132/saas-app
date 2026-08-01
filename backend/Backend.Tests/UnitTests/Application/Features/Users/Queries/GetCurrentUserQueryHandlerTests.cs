using backend.Api.Controllers.Users.DTOs;
using backend.Api.Http;
using backend.Application.Features.Users.Queries;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Repositories;
using Moq;
using NUnit.Framework;

namespace backend.Backend.Tests.UnitTests.Application.Features.Users.Queries;

public sealed class GetCurrentUserQueryHandlerTests
{
  [Test]
  public async Task HandleAsync_ShouldReturnSuccess_WhenRoleIsCompany()
  {
    var query = new GetCurrentUserQuery(
      UserId: Guid.NewGuid()
    );
    var userQueryRepository = new Mock<IUserQueryRepository>();

    var company = new CompanyPrivateDto
    {
      Id = query.UserId,
      Role = RoleType.Company
    };

    userQueryRepository
      .Setup(x =>
        x.GetRoleTypeAsync(
          query.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(RoleType.Company);

    userQueryRepository
      .Setup(x =>
        x.GetCurrentCompanyAsync(
          query.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(company);

    var handler = new GetCurrentUserQueryHandler(
      userQueryRepository.Object
    );

    var result = await handler.HandleAsync(
      query,
      CancellationToken.None
    );

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value, Is.SameAs(company));

    userQueryRepository.Verify(
      x => x.GetRoleTypeAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    userQueryRepository.Verify(
      x => x.GetCurrentCompanyAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    userQueryRepository.Verify(
      x => x.GetCurrentPentesterAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Never
    );
  }

  [Test]
  public async Task HandleAsync_ShouldReturnSuccess_WhenRoleIsPentester()
  {
    var query = new GetCurrentUserQuery(
      UserId: Guid.NewGuid()
    );

    var pentester = new PentesterPrivateDto
    {
      Id = query.UserId,
      Role = RoleType.Pentester
    };

    var userQueryRepository = new Mock<IUserQueryRepository>();

    userQueryRepository
      .Setup(x =>
        x.GetRoleTypeAsync(
          query.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(RoleType.Pentester);
    
    userQueryRepository
    .Setup(x => 
      x.GetCurrentPentesterAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      )
    )
    .ReturnsAsync(pentester);

    var handler = new GetCurrentUserQueryHandler(
      userQueryRepository.Object
    );

    var result = await handler.HandleAsync(
      query,
      CancellationToken.None
    );

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value, Is.SameAs(pentester));

    userQueryRepository.Verify(
      x => x.GetRoleTypeAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    userQueryRepository.Verify(
      x => x.GetCurrentPentesterAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    userQueryRepository.Verify(
      x => x.GetCurrentCompanyAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Never
    );

  }

  [Test]
  public async Task HandleAsync_ShouldReturnFailure_WhenRoleIsNotFound()
  {
    var query = new GetCurrentUserQuery(
      UserId: Guid.NewGuid()
    );
    var userQueryRepository = new Mock<IUserQueryRepository>();

    userQueryRepository
      .Setup(x =>
        x.GetRoleTypeAsync(
          query.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((RoleType)999);

    var handler = new GetCurrentUserQueryHandler(
      userQueryRepository.Object
    );

    var result = await handler.HandleAsync(
      query,
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.General.BadRequest));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));

    userQueryRepository.Verify(
      x => x.GetRoleTypeAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    userQueryRepository.Verify(
      x => x.GetCurrentCompanyAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Never
    );

    userQueryRepository.Verify(
      x => x.GetCurrentPentesterAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Never
    );
  }
}