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
using SportSync.Infrastructure.Jobs;
using Xunit;

namespace SportSync.Infrastructure.Tests.JobsTests
{
    public class CreateAdditionalTerminsJobTests
    {
        private readonly Mock<ITerminRepository> _terminRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<CreateAdditionalTerminsJob>> _loggerMock = new();
        private readonly Mock<IOptions<EventSettings>> _eventSettingsMock = new();

        [Fact]
        public async Task Job_ShouldSucceed_WhenNoTerminsExists()
        {
            _terminRepositoryMock.Setup(r => r.GetLastRepeatableTermins())
                .ReturnsAsync(Enumerable.Empty<(Termin, int)>().ToList());

            var job = new CreateAdditionalTerminsJob(
                _terminRepositoryMock.Object,
                _eventSettingsMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object);

            await job.Execute(new Mock<IJobExecutionContext>().Object);

            _terminRepositoryMock.Verify(r => r.InsertRange(It.IsAny<List<Termin>>()), Times.Never);
            _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            _loggerMock.VerifyLog(LogLevel.Information, "Job 'CreateAdditionalTerminsJob' completed successfully", Times.Once());
        }

        [Fact]
        public async Task Job_ShouldCreate_CorrectNumberOfTermins()
        {
            var @event = Event.Create(
                User.Create("Ante", "Kadic", "ante@gmail.com", "093472836", "jd394fz4398"),
                "event",
                SportType.Football,
                "address",
                12M,
                12,
                string.Empty);

            var tomorrow = DateTime.Today.AddDays(1);
            var schedule = EventSchedule.Create(DayOfWeek.Wednesday, tomorrow, tomorrow.AddHours(10), tomorrow.AddHours(11), true);

            var termin = Termin.Create(@event, tomorrow, schedule);
            termin.Schedule = schedule;
            var returnedLastTermins = new List<(Termin, int)>()
            {
                (termin, 2)
            };

            _eventSettingsMock.Setup(s => s.Value).Returns(new EventSettings() { NumberOfTerminsToCreateInFuture = 5 });

            _terminRepositoryMock.Setup(r => r.GetLastRepeatableTermins())
                .ReturnsAsync(returnedLastTermins);

            var job = new CreateAdditionalTerminsJob(
                _terminRepositoryMock.Object,
                _eventSettingsMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object);

            await job.Execute(new Mock<IJobExecutionContext>().Object);

            _terminRepositoryMock.Verify(r => r.InsertRange(It.Is<List<Termin>>(x => x.Count == 3)), Times.Once);
            _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.VerifyLog(LogLevel.Information, "Job 'CreateAdditionalTerminsJob' completed successfully", Times.Once());
        }

        [Theory]
        [InlineData(TerminStatus.Canceled)]
        [InlineData(TerminStatus.Finished)]
        public async Task Job_ShouldFail_WhenTerminIsNotOpen(TerminStatus status)
        {
            var @event = Event.Create(
                User.Create("Ante", "Kadic", "ante@gmail.com", "093472836", "jd394fz4398"),
                "event",
                SportType.Football,
                "address",
                12M,
                12,
                string.Empty);

            var tomorrow = DateTime.Today.AddDays(1);
            var schedule = EventSchedule.Create(DayOfWeek.Wednesday, tomorrow, tomorrow.AddHours(10), tomorrow.AddHours(11), true);
            var termin = Termin.Create(@event, tomorrow, schedule);

            termin.Status = status;
            termin.Schedule = schedule;
            var returnedLastTermins = new List<(Termin, int)>()
            {
                (termin, 2)
            };

            _eventSettingsMock.Setup(s => s.Value).Returns(new EventSettings() { NumberOfTerminsToCreateInFuture = 5 });

            _terminRepositoryMock.Setup(r => r.GetLastRepeatableTermins())
                .ReturnsAsync(returnedLastTermins);

            var job = new CreateAdditionalTerminsJob(
                _terminRepositoryMock.Object,
                _eventSettingsMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object);

            var exception = await Assert.ThrowsAsync<DomainException>(() => job.Execute(new Mock<IJobExecutionContext>().Object));
            exception.Error.Message.Should().Be(DomainErrors.Termin.NotOpen.Message);

            _terminRepositoryMock.Verify(r => r.InsertRange(It.IsAny<List<Termin>>()), Times.Never);
            _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _loggerMock.VerifyLog(LogLevel.Information, "Job 'CreateAdditionalTerminsJob' completed successfully", Times.Never());
        }

        [Fact]
        public async Task Job_ShouldNotCreate_ForNotRepeatableTermins()
        {
            var @event = Event.Create(
                User.Create("Ante", "Kadic", "ante@gmail.com", "093472836", "jd394fz4398"),
                "event",
                SportType.Football,
                "address",
                12M,
                12,
                string.Empty);
            var tomorrow = DateTime.Today.AddDays(1);
            var schedule = EventSchedule.Create(DayOfWeek.Wednesday, tomorrow, tomorrow.AddHours(10), tomorrow.AddHours(11), false);
            var termin = Termin.Create(@event, tomorrow, schedule);

            termin.Schedule = schedule;
            var returnedLastTermins = new List<(Termin, int)>()
            {
                (termin, 2)
            };

            _eventSettingsMock.Setup(s => s.Value).Returns(new EventSettings() { NumberOfTerminsToCreateInFuture = 5 });

            _terminRepositoryMock.Setup(r => r.GetLastRepeatableTermins())
                .ReturnsAsync(returnedLastTermins);


            var job = new CreateAdditionalTerminsJob(
                _terminRepositoryMock.Object,
                _eventSettingsMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object);

            await job.Execute(new Mock<IJobExecutionContext>().Object);

            _terminRepositoryMock.Verify(r => r.InsertRange(It.IsAny<List<Termin>>()), Times.Never);
            _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _loggerMock.VerifyLog(LogLevel.Information, "Job 'CreateAdditionalTerminsJob' completed successfully", Times.Once());
        }
    }
}