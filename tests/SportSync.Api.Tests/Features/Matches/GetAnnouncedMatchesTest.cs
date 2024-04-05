using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Matches.GetAnnouncedMatches;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class GetAnnouncedMatchesTest : IntegrationTest
{
    [Fact]
    public async Task GetAnnouncedMatches_ShouldReturnOnlyPublic_WhenUserHasNoFriends()
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var requestUser = Database.AddUser();
        var creatorUser = Database.AddUser("creator", "user", "user@gmail.com");

        var publicMatch = Database.AddMatch(creatorUser, startDate: tomorrow);
        var privateMatch = Database.AddMatch(creatorUser, startDate: tomorrow);
        publicMatch.AddPlayer(creatorUser.Id);
        privateMatch.AddPlayer(creatorUser.Id);

        publicMatch.Announce(creatorUser, true, 3, string.Empty);
        privateMatch.Announce(creatorUser, false, 3, string.Empty);

        await Database.UnitOfWork.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query {{
                    announcedMatches(input: {{date: ""{tomorrow.Date}""}}){{
                        matches{{
                            eventName, numberOfPlayersExpected, id
                        }}
                    }}
                }}"));

        var response = result.ToResponseObject<GetAnnouncedMatchResponse>("announcedMatches");
        response.Matches.Count.Should().Be(1);
        response.Matches.Single().Id.Should().Be(publicMatch.Id);
    }

    [Fact]
    public async Task GetAnnouncedMatches_ShouldReturn_PublicAndFromFriendList()
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var requestUser = Database.AddUser();
        var creatorUser = Database.AddUser("creator", "user", "user@gmail.com");
        var friendUser = Database.AddUser("friend", "user", "user@gmail.com");

        var publicMatch = Database.AddMatch(creatorUser, startDate: tomorrow);
        var privateMatch = Database.AddMatch(creatorUser, startDate: tomorrow);
        var privateFromFriendMatch = Database.AddMatch(friendUser, startDate: tomorrow);

        publicMatch.AddPlayers(new List<Guid>() { creatorUser.Id });
        privateMatch.AddPlayers(new List<Guid>() { creatorUser.Id });
        privateFromFriendMatch.AddPlayers(new List<Guid>() { friendUser.Id });

        publicMatch.Announce(creatorUser, true, 3, string.Empty);
        privateMatch.Announce(creatorUser, false, 3, string.Empty);
        privateFromFriendMatch.Announce(friendUser, false, 3, string.Empty);

        Database.AddFriendship(requestUser, friendUser);

        await Database.UnitOfWork.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query {{
                    announcedMatches(input: {{date: ""{tomorrow.Date}""}}){{
                        matches{{
                            eventName, numberOfPlayersExpected, id
                        }}
                    }}
                }}"));

        var response = result.ToResponseObject<GetAnnouncedMatchResponse>("announcedMatches");
        response.Matches.Count.Should().Be(2);
        response.Matches.FirstOrDefault(x => x.Id == publicMatch.Id).Should().NotBeNull();
        response.Matches.FirstOrDefault(x => x.Id == privateFromFriendMatch.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task GetAnnouncedMatches_ShouldReturnMatchOnlyOnce_WhenItHasMultipleAnnouncements()
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var requestUser = Database.AddUser();
        var friendOne = Database.AddUser();
        var friendTwo = Database.AddUser();

        var match = Database.AddMatch(friendOne, startDate: tomorrow);

        match.AddPlayers(new List<Guid>() { friendTwo.Id });
        match.Announce(friendOne, false, 3, string.Empty);
        match.Announce(friendTwo, false, 3, string.Empty);

        Database.AddFriendship(requestUser, friendOne);
        Database.AddFriendship(requestUser, friendTwo);

        await Database.UnitOfWork.SaveChangesAsync();

        Database.DbContext.Set<MatchAnnouncement>().Where(x => x.MatchId == match.Id).Count().Should().Be(2);

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query {{
                    announcedMatches(input: {{date: ""{tomorrow.Date}""}}){{
                        matches{{
                            eventName, numberOfPlayersExpected, id
                        }}
                    }}
                }}"));

        var response = result.ToResponseObject<GetAnnouncedMatchResponse>("announcedMatches");
        response.Matches.Count.Should().Be(1);
        response.Matches.FirstOrDefault(x => x.Id == match.Id).Should().NotBeNull();
    }
}