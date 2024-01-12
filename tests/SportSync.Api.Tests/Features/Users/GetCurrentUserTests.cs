using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Users;

public class GetCurrentUserTests : IntegrationTest
{
    [Fact]
    public async Task Query_Me_ShouldReturnCurrentUser()
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
        var userResponse = result.ToObject<User>("me");

        userResponse.FirstName.Should().Be("Ante");
    }
}
