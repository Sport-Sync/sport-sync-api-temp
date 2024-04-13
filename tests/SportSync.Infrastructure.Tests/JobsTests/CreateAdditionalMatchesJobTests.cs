using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Quartz;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Core.Settings;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;
using SportSync.Infrastructure.Jobs;
using Xunit;
using Match = SportSync.Domain.Entities.Match;

namespace SportSync.Infrastructure.Tests.JobsTests;

public class CreateAdditionalMatchesJobTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<CreateAdditionalMatchesJob>> _loggerMock = new();
    private readonly Mock<IOptions<EventSettings>> _eventSettingsMock = new();

    [Fact]
    public async Task Job_ShouldSucceed_WhenNoMatchesExists()
    {
        _matchRepositoryMock.Setup(r => r.GetLastRepeatableMatches())
            .ReturnsAsync(Enumerable.Empty<(Match, int)>().ToList());

        var job = new CreateAdditionalMatchesJob(
            _matchRepositoryMock.Object,
            _eventSettingsMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);

        await job.Execute(new Mock<IJobExecutionContext>().Object);

        _matchRepositoryMock.Verify(r => r.InsertRange(It.IsAny<List<Match>>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _loggerMock.VerifyLog(LogLevel.Information, "Job 'CreateAdditionalMatchesJob' completed successfully", Times.Once());
    }

    [Fact]
    public async Task Job_ShouldCreate_CorrectNumberOfMatches()
    {
        var @event = Event.Create(
            User.Create("Ante", "Kadic", "ante@gmail.com", PhoneNumber.Create("095472836").Value, "jd394fz4398"),
            "event",
            SportTypeEnum.Football,
            "address",
            12M,
            12,
            string.Empty);

        var tomorrow = DateTime.Today.AddDays(1);
        var schedule = EventSchedule.Create(DayOfWeek.Wednesday, tomorrow, tomorrow.AddHours(10), tomorrow.AddHours(11), true);

        var match = Match.Create(@event, tomorrow, schedule);
        match.Schedule = schedule;
        var returnedLastMatches = new List<(Match, int)>()
        {
            (match, 2)
        };

        _eventSettingsMock.Setup(s => s.Value).Returns(new EventSettings() { NumberOfMatchesToCreateInFuture = 5 });

        _matchRepositoryMock.Setup(r => r.GetLastRepeatableMatches())
            .ReturnsAsync(returnedLastMatches);

        var job = new CreateAdditionalMatchesJob(
            _matchRepositoryMock.Object,
            _eventSettingsMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);

        await job.Execute(new Mock<IJobExecutionContext>().Object);

        _matchRepositoryMock.Verify(r => r.InsertRange(It.Is<List<Match>>(x => x.Count == 3)), Times.Once);
        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Information, "Job 'CreateAdditionalMatchesJob' completed successfully", Times.Once());
    }

    [Theory]
    [InlineData(MatchStatusEnum.Canceled)]
    [InlineData(MatchStatusEnum.Finished)]
    [InlineData(MatchStatusEnum.InProgress)]
    public async Task Job_ShouldFail_WhenMatchIsNotPending(MatchStatusEnum status)
    {
        var @event = Event.Create(
            User.Create("Ante", "Kadic", "ante@gmail.com", PhoneNumber.Create("095472836").Value, "jd394fz4398"),
            "event",
            SportTypeEnum.Football,
            "address",
            12M,
            12,
            string.Empty);

        var tomorrow = DateTime.Today.AddDays(1);
        var schedule = EventSchedule.Create(DayOfWeek.Wednesday, tomorrow, tomorrow.AddHours(10), tomorrow.AddHours(11), true);
        var match = Match.Create(@event, tomorrow, schedule);

        match.Status = status;
        match.Schedule = schedule;
        var returnedLastMatches = new List<(Match, int)>()
        {
            (match, 2)
        };

        _eventSettingsMock.Setup(s => s.Value).Returns(new EventSettings() { NumberOfMatchesToCreateInFuture = 5 });

        _matchRepositoryMock.Setup(r => r.GetLastRepeatableMatches())
            .ReturnsAsync(returnedLastMatches);

        var job = new CreateAdditionalMatchesJob(
            _matchRepositoryMock.Object,
            _eventSettingsMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);

        var exception = await Assert.ThrowsAsync<DomainException>(() => job.Execute(new Mock<IJobExecutionContext>().Object));
        exception.Error.Message.Should().Be(status.ToError());

        _matchRepositoryMock.Verify(r => r.InsertRange(It.IsAny<List<Match>>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyLog(LogLevel.Information, "Job 'CreateAdditionalMatchesJob' completed successfully", Times.Never());
    }

    [Fact]
    public async Task Job_ShouldNotCreate_ForNotRepeatableMatches()
    {
        var @event = Event.Create(
            User.Create("Ante", "Kadic", "ante@gmail.com", PhoneNumber.Create("095472836").Value, "jd394fz4398"),
            "event",
            SportTypeEnum.Football,
            "address",
            12M,
            12,
            string.Empty);
        var tomorrow = DateTime.Today.AddDays(1);
        var schedule = EventSchedule.Create(DayOfWeek.Wednesday, tomorrow, tomorrow.AddHours(10), tomorrow.AddHours(11), false);
        var match = Match.Create(@event, tomorrow, schedule);

        match.Schedule = schedule;
        var returnedLastMatches = new List<(Match, int)>()
        {
            (match, 2)
        };

        _eventSettingsMock.Setup(s => s.Value).Returns(new EventSettings() { NumberOfMatchesToCreateInFuture = 5 });

        _matchRepositoryMock.Setup(r => r.GetLastRepeatableMatches())
            .ReturnsAsync(returnedLastMatches);


        var job = new CreateAdditionalMatchesJob(
            _matchRepositoryMock.Object,
            _eventSettingsMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);

        await job.Execute(new Mock<IJobExecutionContext>().Object);

        _matchRepositoryMock.Verify(r => r.InsertRange(It.IsAny<List<Match>>()), Times.Never);
        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _loggerMock.VerifyLog(LogLevel.Information, "Job 'CreateAdditionalMatchesJob' completed successfully", Times.Once());
    }
}