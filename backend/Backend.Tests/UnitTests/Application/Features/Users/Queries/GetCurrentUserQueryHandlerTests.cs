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
    var mockRepo = new Mock<IUserQueryRepository>();

    var company = new CompanyPrivateDto
    {
      Id = query.UserId,
      Role = RoleType.Company
    };

    mockRepo
      .Setup(x =>
        x.GetRoleTypeAsync(
          query.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(RoleType.Company);

    mockRepo
      .Setup(x =>
        x.GetCurrentCompanyAsync(
          query.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(company);

    var handler = new GetCurrentUserQueryHandler(
      mockRepo.Object
    );

    var result = await handler.HandleAsync(
      query,
      CancellationToken.None
    );

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value, Is.SameAs(company));

    mockRepo.Verify(
      x => x.GetRoleTypeAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockRepo.Verify(
      x => x.GetCurrentCompanyAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockRepo.Verify(
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

    var mockRepo = new Mock<IUserQueryRepository>();

    mockRepo
      .Setup(x =>
        x.GetRoleTypeAsync(
          query.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(RoleType.Pentester);
    
    mockRepo
    .Setup(x => 
      x.GetCurrentPentesterAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      )
    )
    .ReturnsAsync(pentester);

    var handler = new GetCurrentUserQueryHandler(
      mockRepo.Object
    );

    var result = await handler.HandleAsync(
      query,
      CancellationToken.None
    );

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value, Is.SameAs(pentester));

    mockRepo.Verify(
      x => x.GetRoleTypeAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockRepo.Verify(
      x => x.GetCurrentPentesterAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockRepo.Verify(
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
    var mockRepo = new Mock<IUserQueryRepository>();

    mockRepo
      .Setup(x =>
        x.GetRoleTypeAsync(
          query.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((RoleType)999);

    var handler = new GetCurrentUserQueryHandler(
      mockRepo.Object
    );

    var result = await handler.HandleAsync(
      query,
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.General.BadRequest));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));

    mockRepo.Verify(
      x => x.GetRoleTypeAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockRepo.Verify(
      x => x.GetCurrentCompanyAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Never
    );

    mockRepo.Verify(
      x => x.GetCurrentPentesterAsync(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Never
    );
  }
}