using backend.Api.Controllers.Users.DTOs;
using backend.Application.Features.Users.Queries;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Repositories;
using Moq;
using NUnit.Framework;

namespace backend.Backend.Tests.UnitTests.Application.Features.Users.Queries;

public sealed class GetCurrentUserApplicationsQueryHandlerTests
{
  [Test]
  public async Task HandleAsync_ShouldReturnSuccess_WhenApplicationsExist()
  {
    var query = new GetCurrentUserApplicationsQuery(
      UserId: Guid.NewGuid(),
      Status: null
    );
    var applicationsList = new List<UserApplicationsDto>
    {
      new UserApplicationsDto
      {
        ApplicationId = 1,
        ContractId = 1,
        Status = ContractApplicationStatus.Accepted
      }
    };

    var mockRepo = new Mock<IUserQueryRepository>();

    mockRepo
      .Setup(x => 
        x.GetApplicationsAsync(
          query.UserId,
          query.Status,
          It.IsAny<CancellationToken>()
        ))
      .ReturnsAsync(applicationsList);
  

    var handler = new GetCurrentUserApplicationsQueryHandler(
      mockRepo.Object
    );

    var result = await handler.HandleAsync(
      query,
      CancellationToken.None
    );

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value, Is.SameAs(applicationsList));

    mockRepo.Verify(x => 
      x.GetApplicationsAsync(
        query.UserId,
        query.Status,
        It.IsAny<CancellationToken>()
      )
    );
  }
}