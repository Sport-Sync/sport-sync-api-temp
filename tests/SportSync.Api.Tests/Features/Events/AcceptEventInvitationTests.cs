﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using Match = SportSync.Domain.Entities.Match;

namespace SportSync.Api.Tests.Features.Events;

[Collection("IntegrationTests")]
public class AcceptEventInvitationTests : IntegrationTest
{
    [Fact]
    public async Task AcceptEventInvitation_ShouldFail_WhenInvitationNotFound()
    {
        var user = Database.AddUser();
        var ev = Database.AddEvent(user);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptEventInvitation(input: {{ 
                        eventInvitationId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptEventInvitation", DomainErrors.EventInvitation.NotFound);
        Database.DbContext.Set<EventInvitation>().Should().BeEmpty();
    }

    [Fact]
    public async Task AcceptEventInvitation_ShouldFail_WhenCurrentUserIsNotInvitee()
    {
        var eventRepositoryMock = new Mock<IEventRepository>();
        eventRepositoryMock.Setup(x => x.GetPendingInvitations(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<EventInvitation>().ToList);

        var admin = Database.AddUser();
        var invitee = Database.AddUser();
        var curentUser = Database.AddUser();

        var ev = Database.AddEvent(admin);
        var invitation = await ev.InviteUser(admin, invitee, eventRepositoryMock.Object);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(curentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptEventInvitation(input: {{ 
                        eventInvitationId: ""{invitation.Value.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptEventInvitation", DomainErrors.User.Forbidden);
    }

    [Fact]
    public async Task AcceptEventInvitation_ShouldFail_WhenIsAlreadyAccepted()
    {
        var eventRepositoryMock = new Mock<IEventRepository>();
        eventRepositoryMock.Setup(x => x.GetPendingInvitations(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<EventInvitation>().ToList);

        var admin = Database.AddUser();
        var invitee = Database.AddUser();

        var ev = Database.AddEvent(admin);
        var invitation = await ev.InviteUser(admin, invitee, eventRepositoryMock.Object);
        invitation.Value.Accept(DateTime.UtcNow);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(invitee.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptEventInvitation(input: {{ 
                        eventInvitationId: ""{invitation.Value.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptEventInvitation", DomainErrors.EventInvitation.AlreadyAccepted);
    }

    [Fact]
    public async Task AcceptEventInvitation_ShouldFail_WhenIsAlreadyRejected()
    {
        var eventRepositoryMock = new Mock<IEventRepository>();
        eventRepositoryMock.Setup(x => x.GetPendingInvitations(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<EventInvitation>().ToList);

        var admin = Database.AddUser();
        var invitee = Database.AddUser();

        var ev = Database.AddEvent(admin);
        var invitation = await ev.InviteUser(admin, invitee, eventRepositoryMock.Object);
        invitation.Value.Reject(DateTime.UtcNow);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(invitee.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptEventInvitation(input: {{ 
                        eventInvitationId: ""{invitation.Value.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptEventInvitation", DomainErrors.EventInvitation.AlreadyRejected);
    }

    [Fact]
    public async Task AcceptEventInvitation_ShouldSucceed_AndAddMember()
    {
        var now = DateTime.UtcNow;
        DateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

        var eventRepositoryMock = new Mock<IEventRepository>();
        eventRepositoryMock.Setup(x => x.GetPendingInvitations(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<EventInvitation>().ToList);

        var admin = Database.AddUser();
        var invitee = Database.AddUser();

        var ev = Database.AddEvent(admin);
        var invitation = await ev.InviteUser(admin, invitee, eventRepositoryMock.Object);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(invitee.Id);
        Database.DbContext.Set<EventMember>().FirstOrDefault(x => x.UserId == invitee.Id && x.EventId == ev.Id).Should().BeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptEventInvitation(input: {{ 
                        eventInvitationId: ""{invitation.Value.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("acceptEventInvitation");

        var invitationDb = Database.DbContext.Set<EventInvitation>().FirstOrDefault(x => x.Id == invitation.Value.Id);
        invitationDb.Accepted.Should().BeTrue();
        invitationDb.Rejected.Should().BeFalse();
        invitationDb.CompletedOnUtc.Should().Be(now);

        Database.DbContext.Set<EventMember>().FirstOrDefault(x => x.UserId == invitee.Id && x.EventId == ev.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task AcceptEventInvitation_Should_AddPlayersToFutureMatches()
    {
        var now = DateTime.UtcNow;
        DateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

        var eventRepositoryMock = new Mock<IEventRepository>();
        eventRepositoryMock.Setup(x => x.GetPendingInvitations(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<EventInvitation>().ToList);

        var admin = Database.AddUser();
        var invitee = Database.AddUser();

        var ev = Database.AddEvent(admin);
        var pendingMatch = Match.Create(ev, now.AddDays(1), EventSchedule.Create(DayOfWeek.Friday, DateTime.Today, DateTime.Today, DateTime.Today, true));
        var finished = Match.Create(ev, now.AddDays(-1), EventSchedule.Create(DayOfWeek.Friday, DateTime.Today, DateTime.Today, DateTime.Today, true));
        var canceled = Match.Create(ev, now.AddDays(-1), EventSchedule.Create(DayOfWeek.Friday, DateTime.Today, DateTime.Today, DateTime.Today, true));
        var inProgress = Match.Create(ev, now.AddDays(-1), EventSchedule.Create(DayOfWeek.Friday, DateTime.Today, DateTime.Today, DateTime.Today, true));

        pendingMatch.Status = MatchStatusEnum.Pending;
        finished.Status = MatchStatusEnum.Finished;
        canceled.Status = MatchStatusEnum.Canceled;
        inProgress.Status = MatchStatusEnum.InProgress;

        Database.DbContext.Set<Match>().Add(pendingMatch);
        Database.DbContext.Set<Match>().Add(finished);
        Database.DbContext.Set<Match>().Add(canceled);
        Database.DbContext.Set<Match>().Add(inProgress);

        var invitation = await ev.InviteUser(admin, invitee, eventRepositoryMock.Object);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(invitee.Id);
        Database.DbContext.Set<EventMember>().FirstOrDefault(x => x.UserId == invitee.Id && x.EventId == ev.Id).Should().BeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptEventInvitation(input: {{ 
                        eventInvitationId: ""{invitation.Value.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("acceptEventInvitation");

        var matches = Database.DbContext.Set<Match>()
            .Where(m => m.EventId == ev.Id)
            .ToList();

        var futureMatches = matches.Where(x => x.Status == MatchStatusEnum.Pending).Select(x => x.Id);
        var notPendingMatches = matches.Where(x => x.Status != MatchStatusEnum.Pending).Select(x => x.Id);

        notPendingMatches.Count().Should().Be(3);

        var playersInFutureMatches = Database.DbContext.Set<Player>()
            .Where(x => futureMatches.Contains(x.MatchId))
            .ToList();

        var playersInNotPendingMatches = Database.DbContext.Set<Player>()
            .Where(x => notPendingMatches.Contains(x.MatchId))
            .ToList();

        playersInNotPendingMatches.Count().Should().Be(0);
        playersInFutureMatches.Count().Should().Be(1);

        playersInNotPendingMatches.FirstOrDefault(x => x.UserId == invitee.Id).Should().BeNull();
        playersInFutureMatches.FirstOrDefault(x => x.UserId == invitee.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task AcceptEventInvitation_ShouldAddNotificationsForAllMembers_AndCompleteInvitationNotification()
    {
        var now = DateTime.UtcNow;
        DateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

        var eventRepositoryMock = new Mock<IEventRepository>();
        eventRepositoryMock.Setup(x => x.GetPendingInvitations(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<EventInvitation>().ToList);

        var admin = Database.AddUser();
        var member1 = Database.AddUser();
        var member2 = Database.AddUser();
        var invitee = Database.AddUser();

        var ev = Database.AddEvent(admin);
        ev.AddMembers(member1.Id, member2.Id);
        ev.MakeAdmin(member1);

        var invitation = await ev.InviteUser(admin, invitee, eventRepositoryMock.Object);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(invitee.Id);

        var existingNotifications = Database.DbContext.Set<Notification>().ToList();
        existingNotifications.Count.Should().Be(1);
        var inviteeNot = existingNotifications.Single(x => x.UserId == invitee.Id);
        inviteeNot.Type.Should().Be(NotificationTypeEnum.EventInvitationReceived);
        inviteeNot.Completed.Should().BeFalse();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptEventInvitation(input: {{ 
                        eventInvitationId: ""{invitation.Value.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("acceptEventInvitation");

        var invitationDb = Database.DbContext.Set<EventInvitation>().FirstOrDefault(x => x.Id == invitation.Value.Id);
        invitationDb.Accepted.Should().BeTrue();
        invitationDb.Rejected.Should().BeFalse();
        invitationDb.CompletedOnUtc.Should().Be(now);

        var notifications = Database.DbContext.Set<Notification>().ToList();
        notifications.Count.Should().Be(4);

        var inviteeNotification = notifications.First(x => x.UserId == invitee.Id);
        var senderNotification = notifications.First(x => x.UserId == admin.Id);
        var member1Notification = notifications.First(x => x.UserId == member1.Id);
        var member2Notification = notifications.First(x => x.UserId == member2.Id);

        senderNotification.Type.Should().Be(NotificationTypeEnum.EventInvitationAccepted);
        senderNotification.Completed.Should().BeFalse();

        member1Notification.Type.Should().Be(NotificationTypeEnum.MemberJoinedEvent);
        senderNotification.Completed.Should().BeFalse();
        
        member2Notification.Type.Should().Be(NotificationTypeEnum.MemberJoinedEvent);
        senderNotification.Completed.Should().BeFalse();

        inviteeNotification.Type.Should().Be(NotificationTypeEnum.EventInvitationReceived);
        inviteeNotification.Completed.Should().BeTrue();
    }
}