using Moq;
using Microsoft.Extensions.Logging;
using FolhaDePonto.Services;
using FolhaDePonto.Interfaces;
using FolhaDePonto.Entities;
using FolhaDePonto.Exceptions.FolhaDePontoExceptions;

namespace FolhaDePontoTest
{
    public class FolhaDePontoServiceTest
    {
        [Theory]
        [InlineData("2022-01-01T08:00:00")]
        public void ShouldRegisterATimeMoment(string dateTimeStr)
        {
            Mock<ILogger<FolhaDePontoService>> mockLogger = new Mock<ILogger<FolhaDePontoService>>();
            IFolhaDePonto folhaDePonto = new FolhaDePontoService(mockLogger.Object);
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);

            var hourPart = timeMoment.DateTime.ToLongTimeString();

            var result = folhaDePonto.ClockIn(timeMoment);

            Assert.NotNull(result.Hours.FirstOrDefault(x => x.ToLongTimeString() == hourPart));
        }

        
        [Theory]
        [InlineData("2022-01-01T08:00:00")]
        public void ShouldFailWhenRegisterATimeMomentThatAlreadyExists(string dateTimeStr)
        {
            Mock<ILogger<FolhaDePontoService>> mockLogger = new Mock<ILogger<FolhaDePontoService>>();
            IFolhaDePonto folhaDePonto = new FolhaDePontoService(mockLogger.Object);
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);

            folhaDePonto.ClockIn(timeMoment);

            var exceptionCall = () => folhaDePonto.ClockIn(timeMoment);

            Assert.Throws<HourAlreadyExistsException>(exceptionCall);
        }

        [Theory]
        [InlineData("2022-01-01T08:00:00")]
        public void ShouldFailWhenExceedsTimeMomentRegister(string dateTimeStr)
        {
            Mock<ILogger<FolhaDePontoService>> mockLogger = new Mock<ILogger<FolhaDePontoService>>();
            IFolhaDePonto folhaDePonto = new FolhaDePontoService(mockLogger.Object);
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);

            folhaDePonto.ClockIn(timeMoment);

            timeMoment.DateTime = timeMoment.DateTime.AddHours(1);
            folhaDePonto.ClockIn(timeMoment);
            
            timeMoment.DateTime = timeMoment.DateTime.AddHours(1);
            folhaDePonto.ClockIn(timeMoment);
            
            timeMoment.DateTime = timeMoment.DateTime.AddHours(1);
            folhaDePonto.ClockIn(timeMoment);

            var exceptionCall = () => folhaDePonto.ClockIn(timeMoment);

            Assert.Throws<TimeAllocationLimitException>(exceptionCall);
        }

        private TimeMoment TimeMomentArranje(string dateTimeStr)
        {
            TimeMoment timeMoment = new TimeMoment();
            DateTime dateTime = DateTime.Parse(dateTimeStr);
            timeMoment.DateTime = dateTime;
            return timeMoment;
        }
    }
}