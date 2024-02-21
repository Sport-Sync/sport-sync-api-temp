﻿using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Features.Termins;

[Collection("IntegrationTests")]
public class SendTerminApplicationTests : IntegrationTest
{
    [Fact]
    public async Task SendTerminApplication_ShouldFail_WhenUserIsAlreadyPlayer()
    {
        var admin = Database.AddUser();
        var player = Database.AddUser("Player");

        var termin = Database.AddTermin(admin, startDate: DateTime.Today.AddDays(1));
        termin.AddPlayers(new List<Guid>() { player.Id });
        termin.Announce(admin.Id, true);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(player.Id);


        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendTerminApplication(input: {{terminId: ""{termin.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("sendTerminApplication", DomainErrors.TerminApplication.AlreadyPlayer);

        var application = Database.DbContext.Set<TerminApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == player.Id && x.TerminId == termin.Id);

        application.Should().BeNull();
    }

    [Fact]
    public async Task SendTerminApplication_ShouldFail_WhenTerminIsNotAnnounced()
    {
        var admin = Database.AddUser();
        var player = Database.AddUser("Player");

        var termin = Database.AddTermin(admin);
        
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(player.Id);


        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendTerminApplication(input: {{terminId: ""{termin.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("sendTerminApplication", DomainErrors.TerminApplication.NotAnnounced);

        var application = Database.DbContext.Set<TerminApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == player.Id && x.TerminId == termin.Id);

        application.Should().BeNull();
    }

    [Fact]
    public async Task SendTerminApplication_ShouldFail_WhenUserIsNotOnFriendList()
    {
        var admin = Database.AddUser();
        var player = Database.AddUser("Player");
        
        var termin = Database.AddTermin(admin, status: TerminStatus.Pending, startDate: DateTime.Today.AddDays(1));
        termin.Announce(admin.Id, false);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(player.Id);


        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendTerminApplication(input: {{terminId: ""{termin.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("sendTerminApplication", DomainErrors.TerminApplication.NotOnFriendList);

        var application = Database.DbContext.Set<TerminApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == player.Id && x.TerminId == termin.Id);

        application.Should().BeNull();
    }

    [Fact]
    public async Task SendTerminApplication_ShouldSucceed_WhenPublicAnnouncement()
    {
        var admin = Database.AddUser();
        var applicant = Database.AddUser("Player");

        var termin = Database.AddTermin(admin, status: TerminStatus.Pending, startDate:DateTime.Today.AddDays(1));
        termin.Announce(admin.Id, true);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(applicant.Id);


        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendTerminApplication(input: {{terminId: ""{termin.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("sendTerminApplication");

        var application = Database.DbContext.Set<TerminApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == applicant.Id && x.TerminId == termin.Id);

        application.Should().NotBeNull();
        application.Rejected.Should().BeFalse();
        application.Accepted.Should().BeFalse();
        application.CompletedOnUtc.Should().BeNull();
    }

    [Fact]
    public async Task SendTerminApplication_ShouldSucceed_WhenPrivateAnnouncementAndUserIsFriend()
    {
        var admin = Database.AddUser();
        var applicant = Database.AddUser("Player");

        var termin = Database.AddTermin(admin, status: TerminStatus.Pending, startDate: DateTime.Today.AddDays(1));
        Database.AddFriendship(admin, applicant);
        termin.Announce(admin.Id, false);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(applicant.Id);


        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendTerminApplication(input: {{terminId: ""{termin.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("sendTerminApplication");

        var application = Database.DbContext.Set<TerminApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == applicant.Id && x.TerminId == termin.Id);

        application.Should().NotBeNull();
        application.Rejected.Should().BeFalse();
        application.Accepted.Should().BeFalse();
        application.CompletedOnUtc.Should().BeNull();
    }
}