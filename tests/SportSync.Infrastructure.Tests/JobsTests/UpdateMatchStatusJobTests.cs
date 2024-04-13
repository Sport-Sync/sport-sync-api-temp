using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;
using SportSync.Infrastructure.Jobs;
using Xunit;

namespace SportSync.Infrastructure.Tests.JobsTests;

public class UpdateMatchStatusJobTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock = new();
    private readonly Mock<IMatchApplicationRepository> _matchApplicationRepositoryMock = new();
    private readonly Mock<IDateTime> _dateTimeMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<UpdateMatchStatusJob>> _loggerMock = new();

    [Fact]
    public async Task Job_ShouldSucceed_WhenNoMatchesExists()
    {
        var now = new DateTime(2024, 1, 1, 12, 0, 0);
        _dateTimeMock.Setup(x => x.UtcNow).Returns(now);

        var scheduleInOneHour = EventSchedule.Create(DayOfWeek.Wednesday, now.Date, new DateTimeOffset(now.AddHours(1), TimeSpan.Zero), new DateTimeOffset(now.AddHours(2), TimeSpan.Zero), false);
        var scheduleOneHourBeforeNow = EventSchedule.Create(DayOfWeek.Wednesday, now.Date, new DateTimeOffset(now.AddHours(-1), TimeSpan.Zero), new DateTimeOffset(now, TimeSpan.Zero), false);
        var scheduleOneMinuteBeforeNow = EventSchedule.Create(DayOfWeek.Wednesday, now.Date, new DateTimeOffset(now.AddMinutes(-1), TimeSpan.Zero), new DateTimeOffset(now.AddHours(1), TimeSpan.Zero), false);
        var scheduleFiveMinutesBeforeNow = EventSchedule.Create(DayOfWeek.Wednesday, now.Date, new DateTimeOffset(now.AddMinutes(-5), TimeSpan.Zero), new DateTimeOffset(now.AddHours(1), TimeSpan.Zero), false);
        var scheduleInOneMinutes = EventSchedule.Create(DayOfWeek.Wednesday, now.Date, new DateTimeOffset(now.AddMinutes(1), TimeSpan.Zero), new DateTimeOffset(now.AddHours(1), TimeSpan.Zero), false);
        var scheduleInFiveMinutes = EventSchedule.Create(DayOfWeek.Wednesday, now.Date, new DateTimeOffset(now.AddMinutes(5), TimeSpan.Zero), new DateTimeOffset(now.AddHours(1), TimeSpan.Zero), false);
        var scheduleFinishedBeforeFiveMinutes = EventSchedule.Create(DayOfWeek.Wednesday, now.Date, new DateTimeOffset(now.AddHours(-2), TimeSpan.Zero), new DateTimeOffset(now.AddMinutes(-5), TimeSpan.Zero), false);
        var user = User.Create("Michael", "Scott", "michael@gmail.com", PhoneNumber.Create("0987654321").Value, "237854138");
        var ev = Event.Create(user, "Event", SportTypeEnum.Football, "address", 2, 10, null);

        var matchInOneHour = Domain.Entities.Match.Create(ev, now, scheduleInOneHour);
        var matchOneHourBeforeNow = Domain.Entities.Match.Create(ev, now, scheduleOneHourBeforeNow);
        var matchOneMinuteBeforeNow = Domain.Entities.Match.Create(ev, now, scheduleOneMinuteBeforeNow);
        var matchFiveMinutesBeforeNow = Domain.Entities.Match.Create(ev, now, scheduleFiveMinutesBeforeNow);
        var matchInOneMinute = Domain.Entities.Match.Create(ev, now, scheduleInOneMinutes);
        var matchInFiveMinutes = Domain.Entities.Match.Create(ev, now, scheduleInFiveMinutes);
        var matchPendingButFinishedBeforeFiveMinutes = Domain.Entities.Match.Create(ev, now, scheduleFinishedBeforeFiveMinutes);
        var matchInProgressFinishedBeforeFiveMinutes = Domain.Entities.Match.Create(ev, now, scheduleFinishedBeforeFiveMinutes);
        matchInProgressFinishedBeforeFiveMinutes.SetStatus(MatchStatusEnum.InProgress);

        var matchesResponse = new List<Domain.Entities.Match>()
        {
            matchInOneHour,
            matchOneHourBeforeNow,
            matchOneMinuteBeforeNow,
            matchFiveMinutesBeforeNow,
            matchInOneMinute,
            matchInFiveMinutes,
            matchPendingButFinishedBeforeFiveMinutes,
            matchInProgressFinishedBeforeFiveMinutes
        }.AsQueryable();

        _matchRepositoryMock
            .Setup(r => r.Where(It.IsAny<Expression<Func<Domain.Entities.Match, bool>>>()))
            .Returns(matchesResponse);

        _matchApplicationRepositoryMock
            .Setup(x => x.GetPendingByMatchesIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MatchApplication>());

        var job = new UpdateMatchStatusJob(
            _matchRepositoryMock.Object,
            _matchApplicationRepositoryMock.Object,
            _dateTimeMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);

        await job.Execute(new Mock<IJobExecutionContext>().Object);

        matchInOneHour.Status.Should().Be(MatchStatusEnum.Pending);
        matchOneMinuteBeforeNow.Status.Should().Be(MatchStatusEnum.InProgress);
        matchOneHourBeforeNow.Status.Should().Be(MatchStatusEnum.InProgress);
        matchFiveMinutesBeforeNow.Status.Should().Be(MatchStatusEnum.InProgress);
        matchInOneMinute.Status.Should().Be(MatchStatusEnum.Pending);
        matchInFiveMinutes.Status.Should().Be(MatchStatusEnum.Pending);
        matchPendingButFinishedBeforeFiveMinutes.Status.Should().Be(MatchStatusEnum.InProgress);
        matchInProgressFinishedBeforeFiveMinutes.Status.Should().Be(MatchStatusEnum.Finished);

        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _loggerMock.VerifyLog(LogLevel.Information, "Job 'UpdateMatchStatusJob' completed successfully", Times.Once());
    }
}