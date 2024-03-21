using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Features.Events;

[Collection("IntegrationTests")]
public class SendEventInvitationTests : IntegrationTest
{
    [Fact]
    public async Task SendEventInvitation_ShouldFail_WhenSenderIsNotAdmin()
    {
        var admin = Database.AddUser();
        var sender = Database.AddUser("Sender");
        var invitee = Database.AddUser("Invitee");

        var @event = Database.AddEvent(admin);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(sender.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendEventInvitation(input: {{eventId: ""{@event.Id}"", userId: ""{invitee.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("sendEventInvitation", DomainErrors.User.Forbidden);

        var invitation = Database.DbContext.Set<EventInvitation>()
            .FirstOrDefault(x => x.SentToUserId == invitee.Id);

        invitation.Should().BeNull();
    }

    [Fact]
    public async Task SendEventInvitation_ShouldFail_WhenReceiverIsAlreadyMember()
    {
        var admin = Database.AddUser();
        var invitee = Database.AddUser("Invitee");

        var @event = Database.AddEvent(admin);
        @event.AddMembers(invitee.Id);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendEventInvitation(input: {{eventId: ""{@event.Id}"", userId: ""{invitee.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("sendEventInvitation", DomainErrors.EventInvitation.AlreadyMember);

        var invitation = Database.DbContext.Set<EventInvitation>()
            .FirstOrDefault(x => x.SentToUserId == invitee.Id);

        invitation.Should().BeNull();
    }

    [Fact]
    public async Task SendEventInvitation_ShouldFail_WhenReceiverIsAlreadyInvited()
    {
        var admin = Database.AddUser();
        var invitee = Database.AddUser("Invitee");

        var @event = Database.AddEvent(admin);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendEventInvitation(input: {{eventId: ""{@event.Id}"", userId: ""{invitee.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("sendEventInvitation");

        var invitation = Database.DbContext.Set<EventInvitation>()
            .FirstOrDefault(x => x.SentToUserId == invitee.Id);

        invitation.Should().NotBeNull();

        invitation.SentByUserId.Should().Be(admin.Id);
        invitation.Rejected.Should().BeFalse();
        invitation.Accepted.Should().BeFalse();
        invitation.CompletedOnUtc.Should().BeNull();

        Database.DbContext.Set<Notification>()
            .FirstOrDefault(x => x.UserId == invitee.Id)
            .Should().NotBeNull();

        var secondResult = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendEventInvitation(input: {{eventId: ""{@event.Id}"", userId: ""{invitee.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        secondResult.ShouldBeFailureResult("sendEventInvitation", DomainErrors.EventInvitation.AlreadyInvited);
    }

    [Fact]
    public async Task SendEventInvitation_ShouldSucceed_AndCreateNotification()
    {
        var admin = Database.AddUser();
        var invitee = Database.AddUser("Invitee");

        var @event = Database.AddEvent(admin);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendEventInvitation(input: {{eventId: ""{@event.Id}"", userId: ""{invitee.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("sendEventInvitation");

        var invitation = Database.DbContext.Set<EventInvitation>()
            .FirstOrDefault(x => x.SentToUserId == invitee.Id);

        invitation.Should().NotBeNull();

        invitation.SentByUserId.Should().Be(admin.Id);
        invitation.Rejected.Should().BeFalse();
        invitation.Accepted.Should().BeFalse();
        invitation.CompletedOnUtc.Should().BeNull();

        var notification = Database.DbContext.Set<Notification>()
            .FirstOrDefault(x => x.UserId == invitee.Id);

        notification.Should().NotBeNull();
        notification.Completed.Should().BeFalse();
        notification.CompletedOnUtc.Should().BeNull();
        notification.ResourceId.Should().Be(@event.Id);
        notification.Type.Should().Be(NotificationTypeEnum.EventInvitationReceived);
        notification.ContentData.Data.Should().BeEquivalentTo(admin.FullName, @event.Name);
    }
}