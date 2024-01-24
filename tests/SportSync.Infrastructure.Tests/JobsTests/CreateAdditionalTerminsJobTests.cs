using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Core.Settings;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Infrastructure.Jobs;
using Xunit;

namespace SportSync.Infrastructure.Tests.JobsTests
{
    public class CreateAdditionalTerminsJobTests
    {
        private readonly Mock<ITerminRepository> _terminRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateAdditionalTerminsJob>> _loggerMock;
        private readonly Mock<IOptions<EventSettings>> _eventSettingsMock;

        public CreateAdditionalTerminsJobTests()
        {
            _terminRepositoryMock = new Mock<ITerminRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateAdditionalTerminsJob>>();
            _eventSettingsMock = new Mock<IOptions<EventSettings>>();
        }

        [Fact]
        public void Job_ShouldSucceed_WhenNoTerminsExists()
        {
            _terminRepositoryMock.Setup(r => r.GetLastRepeatableTermins())
                .ReturnsAsync(Enumerable.Empty<(Termin, int)>().ToList());

            var job = new CreateAdditionalTerminsJob(
                _terminRepositoryMock.Object,
                _eventSettingsMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object);

            _terminRepositoryMock.Verify(r => r.InsertRange(It.IsAny<List<Termin>>()), Times.Never);
        }

        [Fact]
        public void Job_ShouldCreate_CoorrectNumberOfTermins()
        {
            var @event = Event.Create(
                User.Create("Ante", "Kadiæ", "ante@gmail.com", "093472836", "jd394fz4398"),
                "event",
                SportType.Football,
                "address",
                12M,
                12,
                string.Empty);

            var schedule = EventSchedule.Create(DayOfWeek.Wednesday, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), TimeOnly.Parse("19:00"),
                TimeOnly.Parse("20:00"), true);

            var returnedLastTermins = new List<(Termin, int)>()
            {
                (Termin.Create(@event, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), schedule), 2)
            };

            _eventSettingsMock.Setup(s => s.Value.NumberOfTerminsToCreateInFuture).Returns(5);

            _terminRepositoryMock.Setup(r => r.GetLastRepeatableTermins())
                .ReturnsAsync(returnedLastTermins);

            var job = new CreateAdditionalTerminsJob(
                _terminRepositoryMock.Object,
                _eventSettingsMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object);
        }
    }
}