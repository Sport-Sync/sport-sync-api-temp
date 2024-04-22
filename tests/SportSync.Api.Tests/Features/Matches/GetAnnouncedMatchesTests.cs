using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Matches.GetAnnouncedMatches;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class GetAnnouncedMatchesTests : IntegrationTest
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
                            matchId
                        }}
                    }}
                }}"));

        Database.DbContext.Set<Player>().First(x => x.UserId == creatorUser.Id && x.MatchId == privateMatch.Id).HasAnnouncedMatch.Should().BeTrue();
        Database.DbContext.Set<Player>().First(x => x.UserId == creatorUser.Id && x.MatchId == publicMatch.Id).HasAnnouncedMatch.Should().BeTrue();

        var response = result.ToResponseObject<GetAnnouncedMatchesResponse>("announcedMatches");
        response.Matches.Count.Should().Be(1);
        response.Matches.Single().MatchId.Should().Be(publicMatch.Id);
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
                            matchId
                        }}
                    }}
                }}"));

        var response = result.ToResponseObject<GetAnnouncedMatchesResponse>("announcedMatches");
        response.Matches.Count.Should().Be(2);
        response.Matches.FirstOrDefault(x => x.MatchId == publicMatch.Id).Should().NotBeNull();
        response.Matches.FirstOrDefault(x => x.MatchId == privateFromFriendMatch.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task GetAnnouncedMatches_ShouldReturn_OnlyForFriendsWithAnnouncerFlagTrue()
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var requestUser = Database.AddUser();
        var nonFriendUser = Database.AddUser("creator", "user", "user@gmail.com");
        var friendUser = Database.AddUser("friend", "user", "user@gmail.com");

        var privateMatchNonFriend = Database.AddMatch(nonFriendUser, startDate: tomorrow);
        var privateMatchFriend = Database.AddMatch(friendUser, startDate: tomorrow);
        var privateMatchBothUsers = Database.AddMatch(nonFriendUser, startDate: tomorrow);
        var privateMatchBothUsersAnnouncedByFriend = Database.AddMatch(nonFriendUser, startDate: tomorrow);

        privateMatchBothUsers.AddPlayer(friendUser.Id);
        privateMatchBothUsersAnnouncedByFriend.AddPlayer(friendUser.Id);

        privateMatchNonFriend.Announce(nonFriendUser, false, 2, string.Empty);
        privateMatchFriend.Announce(friendUser, false, 3, string.Empty);
        privateMatchBothUsers.Announce(nonFriendUser, false, 4, string.Empty);
        privateMatchBothUsersAnnouncedByFriend.Announce(friendUser, false, 6, string.Empty);

        Database.AddFriendship(requestUser, friendUser);

        await Database.UnitOfWork.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query {{
                    announcedMatches(input: {{date: ""{tomorrow.Date}""}}){{
                        matches{{
                            matchId, playerLimit
                        }}
                    }}
                }}"));

        var response = result.ToResponseObject<GetAnnouncedMatchesResponse>("announcedMatches");
        response.Matches.Count.Should().Be(2);
        response.Matches.FirstOrDefault(x => x.MatchId == privateMatchFriend.Id).Should().NotBeNull();
        response.Matches.FirstOrDefault(x => x.MatchId == privateMatchFriend.Id).PlayerLimit.Should().Be(3);
        response.Matches.FirstOrDefault(x => x.MatchId == privateMatchBothUsersAnnouncedByFriend.Id).Should().NotBeNull();
        response.Matches.FirstOrDefault(x => x.MatchId == privateMatchBothUsersAnnouncedByFriend.Id).PlayerLimit.Should().Be(6);
    }
}