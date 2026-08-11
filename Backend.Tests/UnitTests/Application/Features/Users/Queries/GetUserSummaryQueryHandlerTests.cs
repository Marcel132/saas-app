using backend.Api.Controllers.Users.DTOs;
using backend.Application.Features.Users.Queries;
using backend.Domain.Interfaces.Repositories;
using Moq;

namespace Backend.Tests.UnitTests.Application.Features.Users.Queries;

public sealed class GetUserSummaryQueryHandlerTests
{
  [Test] 
  public async Task HandleAsync_ShouldReturnUserSummary_WhenUserExists()
  {
    var query = new GetUserSummaryQuery(
      UserId: Guid.NewGuid()
    );

    var expectedSummary = new UserSummaryDto
    {
      ActiveOrders = 5,
      ActiveTasks = 3,
      CompletedReports = 10,
      TotalReports = 15
    };

    var mockRepo = new Mock<IUserQueryRepository>();

    mockRepo
      .Setup(x => 
        x.GetSummary(
          query.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(expectedSummary);

    var handler = new GetUserSummaryQueryHandler(
      mockRepo.Object
    );

    var result = await handler.HandleAsync(
      query,
      CancellationToken.None
    );

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value.ActiveOrders, Is.EqualTo(expectedSummary.ActiveOrders));
    Assert.That(result.Value.ActiveTasks, Is.EqualTo(expectedSummary.ActiveTasks));
    Assert.That(result.Value.CompletedReports, Is.EqualTo(expectedSummary.CompletedReports));
    Assert.That(result.Value.TotalReports, Is.EqualTo(expectedSummary.TotalReports));

    mockRepo.Verify(
      x => x.GetSummary(
        query.UserId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
  }
}