﻿using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class AcceptMatchApplicationTests : IntegrationTest
{
    [Fact]
    public async Task AcceptMatchApplication_ShouldFail_WhenMatchApplicationNotFound()
    {
        var user = Database.AddUser();
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptMatchApplication", DomainErrors.MatchApplication.NotFound);
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldFail_WhenUserIsNotAdmin()
    {
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldHaveError(DomainErrors.User.Forbidden);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeFalse();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.CompletedOnUtc.Should().BeNull();
        matchApplication.CompletedByUserId.Should().BeNull();
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldFail_WhenApplicationIsAlreadyAccepted()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);
        
        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();
        application.Accept(adminOnEvent, match, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptMatchApplication", DomainErrors.MatchApplication.AlreadyAccepted);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeTrue();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.Canceled.Should().BeFalse();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldFail_WhenApplicationIsAlreadyRejected()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();
        application.Reject(adminOnEvent, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptMatchApplication", DomainErrors.MatchApplication.AlreadyRejected);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeFalse();
        matchApplication.Canceled.Should().BeFalse();
        matchApplication.Rejected.Should().BeTrue();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldFail_WhenApplicationIsAlreadyCanceled()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();
        application.Cancel(applicant, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptMatchApplication", DomainErrors.MatchApplication.AlreadyCanceled);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeFalse();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.Canceled.Should().BeTrue();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(applicant.Id);
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldFail_WhenPlayerLimitHasBeenReached()
    {
        var completedTime = DateTime.UtcNow;
        DateTimeProviderMock.Setup(d => d.UtcNow).Returns(completedTime);
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var applicant2 = Database.AddUser("applicant2");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 1, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);
        var secondApplication = Database.AddMatchApplication(applicant2, match);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var initialApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        initialApplication.Accepted.Should().BeFalse();
        initialApplication.Rejected.Should().BeFalse();
        initialApplication.CompletedOnUtc.Should().BeNull();
        initialApplication.CompletedByUserId.Should().BeNull();

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().BeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("acceptMatchApplication");

        var result2 = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{secondApplication.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result2.ShouldBeFailureResult("acceptMatchApplication", DomainErrors.MatchApplication.PlayersLimitReached);

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().NotBeNull();

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant2.Id && x.MatchId == match.Id)
            .Should().BeNull();
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldDeleteAnnouncement_WhenPlayerLimitHasBeenReached()
    {
        var completedTime = DateTime.UtcNow;
        DateTimeProviderMock.Setup(d => d.UtcNow).Returns(completedTime);
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var applicant2 = Database.AddUser("applicant2");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 2, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);
        var secondApplication = Database.AddMatchApplication(applicant2, match);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var initialApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        initialApplication.Accepted.Should().BeFalse();
        initialApplication.Rejected.Should().BeFalse();
        initialApplication.CompletedOnUtc.Should().BeNull();
        initialApplication.CompletedByUserId.Should().BeNull();

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().BeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("acceptMatchApplication");

        Database.DbContext.Set<MatchAnnouncement>().FirstOrDefault(x => x.MatchId == match.Id).Deleted.Should().BeFalse();
        Database.DbContext.Set<MatchAnnouncement>().FirstOrDefault(x => x.MatchId == match.Id).AcceptedPlayersCount.Should().Be(1);

        var result2 = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{secondApplication.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result2.ShouldBeSuccessResult("acceptMatchApplication");

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().NotBeNull();

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant2.Id && x.MatchId == match.Id)
            .Should().NotBeNull();

        Database.DbContext.Set<MatchAnnouncement>().FirstOrDefault(x => x.MatchId == match.Id).Should().BeNull();
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldSucceed_WhenUserIsAdmin()
    {
        var completedTime = DateTime.UtcNow;
        DateTimeProviderMock.Setup(d => d.UtcNow).Returns(completedTime);
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var initialApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        initialApplication.Accepted.Should().BeFalse();
        initialApplication.Rejected.Should().BeFalse();
        initialApplication.CompletedOnUtc.Should().BeNull();
        initialApplication.CompletedByUserId.Should().BeNull();

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().BeNull();
        
        Database.DbContext.Set<MatchAnnouncement>().FirstOrDefault(x => x.MatchId == match.Id).AcceptedPlayersCount.Should().Be(0);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("acceptMatchApplication");
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeTrue();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().NotBeNull();
     
        Database.DbContext.Set<MatchAnnouncement>().FirstOrDefault(x => x.MatchId == match.Id).AcceptedPlayersCount.Should().Be(1);
    }

    [Fact]
    public async Task AcceptMatchApplication_Should_CompleteNotification()
    {
        var completedTime = DateTime.UtcNow;
        DateTimeProviderMock.Setup(d => d.UtcNow).Returns(completedTime);
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminThatHasCompleted = Database.AddUser("admin");
        var admin2 = Database.AddUser("admin");
        var match = Database.AddMatch(adminThatHasCompleted, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminThatHasCompleted, true, 3, string.Empty);

        await Database.SaveChangesAsync();

        var @event = Database.DbContext.Set<Event>().FirstOrDefault();
        @event.AddMembers(admin2.Id);
        @event.MakeAdmin(admin2);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(applicant.Id);

        var applicationResult = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendMatchApplication(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        applicationResult.ShouldBeSuccessResult("sendMatchApplication");

        var application = Database.DbContext.Set<MatchApplication>().Single();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminThatHasCompleted.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("acceptMatchApplication");
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeTrue();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminThatHasCompleted.Id);

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().NotBeNull();

        var notificaitons = Database.DbContext.Set<Notification>().Where(x => x.Type == NotificationTypeEnum.MatchApplicationReceived).ToList();
        notificaitons.Count().Should().Be(2);

        notificaitons.First(x => x.UserId == adminThatHasCompleted.Id).Completed.Should().BeTrue();
        notificaitons.First(x => x.UserId == admin2.Id).Completed.Should().BeFalse();
    }

    [Fact]
    public async Task AcceptMatchApplication_Should_CreateNotificationForApplicant()
    {
        var completedTime = DateTime.UtcNow;
        DateTimeProviderMock.Setup(d => d.UtcNow).Returns(completedTime);
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var initialApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        initialApplication.Accepted.Should().BeFalse();
        initialApplication.Rejected.Should().BeFalse();
        initialApplication.CompletedOnUtc.Should().BeNull();
        initialApplication.CompletedByUserId.Should().BeNull();

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().BeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("acceptMatchApplication");
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeTrue();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().NotBeNull();

        var notification = Database.DbContext.Set<Notification>().FirstOrDefault(x => x.UserId == applicant.Id);
        notification.Should().NotBeNull();
        notification.Type.Should().Be(NotificationTypeEnum.MatchApplicationAccepted);
        notification.Completed.Should().BeFalse();
        notification.ResourceId.Should().Be(match.Id);
    }
}