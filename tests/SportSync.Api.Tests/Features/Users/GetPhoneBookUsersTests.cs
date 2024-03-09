using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Users.GetByPhoneNumbers;

namespace SportSync.Api.Tests.Features.Users;

[Collection("IntegrationTests")]
public class GetPhoneBookUsersTests : IntegrationTest
{
    [Fact]
    public async Task GetByPhoneNumbers_ShouldReturn_WhenFound()
    {
        var currentUser = Database.AddUser();
        var user1 = Database.AddUser(phone: "0919279259");
        var user2 = Database.AddUser(phone: "099927 9259");
        var user3 = Database.AddUser(phone: "+385 99927 3333");
        var user4 = Database.AddUser(phone: "0929279259");

        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"
                query{
                    phoneBookUsers(input: {phoneNumbers: [""0959279259"", ""0919279259"", ""099927 9259"", ""+385 99927 3333""]}){
                        users{
                            firstName, email, phone, id
                        }
                    }
                }"));

        var userResponse = result.ToResponseObject<GetPhoneBookUsersResponse>("phoneBookUsers");

        userResponse.Users.Count.Should().Be(3);
        userResponse.Users.FirstOrDefault(x => x.Id == user1.Id).Should().NotBeNull();
        userResponse.Users.FirstOrDefault(x => x.Id == user2.Id).Should().NotBeNull();
        userResponse.Users.FirstOrDefault(x => x.Id == user3.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task GetByPhoneNumbers_Should_SetPendingFriendshipRequestFlag()
    {
        var currentUser = Database.AddUser();
        var user1 = Database.AddUser(phone: "0919279259");
        var user2 = Database.AddUser(phone: "099927 9259");
        var user3 = Database.AddUser(phone: "+385 99927 3333");
        var user4 = Database.AddUser(phone: "0929279259");
        var user5 = Database.AddUser(phone: "0929279251");

        await Database.SaveChangesAsync();

        Database.AddFriendshipRequest(currentUser, user1);
        Database.AddFriendshipRequest(user2, currentUser);
        Database.AddFriendshipRequest(user3, currentUser, accepted: true);
        Database.AddFriendshipRequest(user4, currentUser, rejected: true);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"
                query{
                    phoneBookUsers(input: {phoneNumbers: [""0929279259"", ""0919279259"", ""099927 9259"", ""0929279251"", ""+385 99927 3333""]}){
                        users{
                            firstName, email, phone, id, pendingFriendshipRequest
                        }
                    }
                }"));

        var userResponse = result.ToResponseObject<GetPhoneBookUsersResponse>("phoneBookUsers");

        userResponse.Users.Count.Should().Be(4);
        userResponse.Users.FirstOrDefault(x => x.Id == user1.Id).PendingFriendshipRequest.Should().BeTrue();
        userResponse.Users.FirstOrDefault(x => x.Id == user2.Id).PendingFriendshipRequest.Should().BeTrue();
        userResponse.Users.FirstOrDefault(x => x.Id == user4.Id).PendingFriendshipRequest.Should().BeFalse();
        userResponse.Users.FirstOrDefault(x => x.Id == user5.Id).PendingFriendshipRequest.Should().BeFalse();

    }

    [Fact]
    public async Task GetByPhoneNumbers_Should_FilterOutFriends()
    {
        var currentUser = Database.AddUser();
        var user1 = Database.AddUser(phone: "0919279259");
        var user2 = Database.AddUser(phone: "099927 9259");
        var user3 = Database.AddUser(phone: "+385 99927 3333");
        var user4 = Database.AddUser(phone: "0929279259");

        currentUser.AddFriend(user1);
        user2.AddFriend(currentUser);

        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"
                query{
                    phoneBookUsers(input: {phoneNumbers: [""0929279259"", ""0919279259"", ""099927 9259"", ""+385 99927 3333""]}){
                        users{
                            firstName, email, phone, id
                        }
                    }
                }"));

        var userResponse = result.ToResponseObject<GetPhoneBookUsersResponse>("phoneBookUsers");

        userResponse.Users.Count.Should().Be(2);
        userResponse.Users.FirstOrDefault(x => x.Id == user3.Id).Should().NotBeNull();
        userResponse.Users.FirstOrDefault(x => x.Id == user4.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task GetByPhoneNumbers_Should_NotReturnCurrentUser()
    {
        var currentUser = Database.AddUser(phone: "0918877665");
        var anotherUser = Database.AddUser(phone: "0959279259");

        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"
                query{
                    phoneBookUsers(input: {phoneNumbers: [""0959279259"", ""0918877665""]}){
                        users{
                            firstName, email, phone, id
                        }
                    }
                }"));

        var userResponse = result.ToResponseObject<GetPhoneBookUsersResponse>("phoneBookUsers");

        userResponse.Users.Count.Should().Be(1);
        userResponse.Users.FirstOrDefault(x => x.Id == anotherUser.Id).Should().NotBeNull();
        userResponse.Users.FirstOrDefault(x => x.Id == currentUser.Id).Should().BeNull();
    }

    [Theory]
    [InlineData("0998026836", "0998026836")]
    [InlineData("+385998026836", "0998026836")]
    [InlineData("+385 9980 26836", "0998026836")]
    [InlineData("(+385) 9980 26836", "0998026836")]
    [InlineData("099-8026-836", "0998026836")]
    [InlineData("099/8026/836", "0998026836")]
    public async Task GetByPhoneNumbers_ShouldSanitize_ThenReturnExistingUsers(string inputPhoneNumber, string existingUserPhoneNumber)
    {
        var currentUser = Database.AddUser();
        var user1 = Database.AddUser(phone: existingUserPhoneNumber);

        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    phoneBookUsers(input: {{phoneNumbers: [""{inputPhoneNumber}""]}}){{
                        users{{
                            firstName, email, phone, id
                        }}
                    }}
                }}"));

        var userResponse = result.ToResponseObject<GetPhoneBookUsersResponse>("phoneBookUsers");

        userResponse.Users.Count.Should().Be(1);
        userResponse.Users.FirstOrDefault(x => x.Id == user1.Id).Should().NotBeNull();
    }
}