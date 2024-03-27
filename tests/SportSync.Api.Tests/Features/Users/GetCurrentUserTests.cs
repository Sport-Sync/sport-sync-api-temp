using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Types;

namespace SportSync.Api.Tests.Features.Users;

[Collection("IntegrationTests")]
public class GetCurrentUserTests : IntegrationTest
{
    [Fact]
    public async Task QueryMe_ShouldReturnCurrentUser_WhenFoundById()
    {
        var user = Database.AddUser("Ante", "Kadić", "ante.kadic@gmail.com", "092374342", "98637286423");
        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"query{
                me{
                    firstName
                }
            }"));

        var userResponse = result.ToResponseObject<UserType>("me");

        userResponse.FirstName.Should().Be("Ante");
    }

    [Fact]
    public async Task QueryMe_ShouldReturnNotFound_WhenNotFoundById()
    {
        UserIdentifierMock.Setup(x => x.UserId).Returns(Guid.NewGuid);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"query{
                me{
                    firstName
                }
            }"));

        result.ShouldHaveError(DomainErrors.User.NotFound);
    }
}
