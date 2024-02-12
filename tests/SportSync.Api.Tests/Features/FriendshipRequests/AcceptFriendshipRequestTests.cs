using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;

namespace SportSync.Api.Tests.Features.FriendshipRequests;

[Collection("IntegrationTests")]
public class AcceptFriendshipRequestTests : IntegrationTest
{
    [Fact]
    public async Task AcceptFriendshipRequest_ShouldFail_WhenFriendshipRequestNotFound()
    {
        var user = Database.AddUser();
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendFriendshipRequest(input: {{ 
                        userId: ""{user.Id}"", friendId: ""{user.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("sendFriendshipRequest", DomainErrors.FriendshipRequest.InvalidSameUserId);
        //Database.DbContext.Set<FriendshipRequest>().FirstOrDefault(x => x.UserId == user.Id && x.FriendId == user.Id).Should().BeNull();
    }
}