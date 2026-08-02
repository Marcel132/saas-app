using backend.Api.Controllers.Users.DTOs;
using backend.Application.Features.Users.Queries;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Repositories;
using Moq;
using NUnit.Framework;

namespace backend.Backend.Tests.UnitTests.Application.Features.Users.Queries;

public sealed class GetCurrentUserContractsQueryHandlerTests
{
  [Test]
  public async Task HandleAsync_ShouldReturnSuccess_WhenContractsExist()
  {
    var query = new GetCurrentUserContractsQuery(
      UserId: Guid.NewGuid(),
      Status: null
    );

    var contractsList = new List<UserContractsDto>
    {
      new UserContractsDto
      {
        ContractId = 1,
        ContractStatus = ContractStatus.Open
      }
    };

    var userQueryRepository = new Mock<IUserQueryRepository>();

    userQueryRepository
      .Setup(x => 
        x.GetCurrentUserContractsAsync(
          query.UserId,
          query.Status,
          It.IsAny<CancellationToken>()
        ))
      .ReturnsAsync(contractsList);
  

    var handler = new GetCurrentUserContractsQueryHandler(
      userQueryRepository.Object
    );

    var result = await handler.HandleAsync(
      query,
      CancellationToken.None
    );

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value, Is.SameAs(contractsList));

    userQueryRepository.Verify(x => 
      x.GetCurrentUserContractsAsync(
        query.UserId,
        query.Status,
        It.IsAny<CancellationToken>()
      )
    );
  }
}