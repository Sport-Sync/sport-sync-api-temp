using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Termins.GetAnnouncedTermins;

namespace SportSync.Api.Tests.Features.Termins;

[Collection("IntegrationTests")]
public class GetAnnouncedTerminsTest : IntegrationTest
{
    [Fact]
    public async Task GetAnnouncedTermins_ShouldReturnOnlyPublic_WhenUserHasNoFriends()
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var requestUser = Database.AddUser();
        var creatorUser = Database.AddUser("creator", "user", "user@gmail.com");

        var publicTermin = Database.AddTermin(creatorUser, startDate: tomorrow);
        var privateTermin = Database.AddTermin(creatorUser, startDate: tomorrow);
        publicTermin.AddPlayer(creatorUser.Id);
        privateTermin.AddPlayer(creatorUser.Id);

        publicTermin.Announce(creatorUser.Id, true);
        privateTermin.Announce(creatorUser.Id, false);

        await Database.UnitOfWork.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query {{
                    announcedTermins(input: {{date: ""{tomorrow.Date}""}}){{
                        termins{{
                            eventName, numberOfPlayersExpected, id
                        }}
                    }}
                }}"));

        var response = result.ToResponseObject<GetAnnouncedTerminResponse>("announcedTermins");
        response.Termins.Count.Should().Be(1);
        response.Termins.Single().Id.Should().Be(publicTermin.Id);
    }

    [Fact]
    public async Task GetAnnouncedTermins_ShouldReturn_PublicAndFromFriendList()
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var requestUser = Database.AddUser();
        var creatorUser = Database.AddUser("creator", "user", "user@gmail.com");
        var friendUser = Database.AddUser("friend", "user", "user@gmail.com");

        var publicTermin = Database.AddTermin(creatorUser, startDate: tomorrow);
        var privateTermin = Database.AddTermin(creatorUser, startDate: tomorrow);
        var privateFromFriendTermin = Database.AddTermin(friendUser, startDate: tomorrow);

        publicTermin.AddPlayers(new List<Guid>() { creatorUser.Id });
        privateTermin.AddPlayers(new List<Guid>() { creatorUser.Id });
        privateFromFriendTermin.AddPlayers(new List<Guid>() { friendUser.Id });

        publicTermin.Announce(creatorUser.Id, true);
        privateTermin.Announce(creatorUser.Id, false);
        privateFromFriendTermin.Announce(friendUser.Id, false);

        Database.AddFriendship(requestUser, friendUser);

        await Database.UnitOfWork.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query {{
                    announcedTermins(input: {{date: ""{tomorrow.Date}""}}){{
                        termins{{
                            eventName, numberOfPlayersExpected, id
                        }}
                    }}
                }}"));

        var response = result.ToResponseObject<GetAnnouncedTerminResponse>("announcedTermins");
        response.Termins.Count.Should().Be(2);
        response.Termins.FirstOrDefault(x => x.Id == publicTermin.Id).Should().NotBeNull();
        response.Termins.FirstOrDefault(x => x.Id == privateFromFriendTermin.Id).Should().NotBeNull();
    }
}