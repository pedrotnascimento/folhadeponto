using Moq;
using Microsoft.Extensions.Logging;
using FolhaDePonto.Exceptions.FolhaDePontoExceptions;
using Domain.Entities;
using Domain.Services;
using Domain.Interfaces;
using Repository.Repositories;

namespace FolhaDePontoTest
{
    public class FolhaDePontoServiceTest
    {
        IFolhaDePonto folhaDePonto;
        public FolhaDePontoServiceTest()
        {
            folhaDePonto = FolhaDePontoServiceArranje();
        }

        [Theory]
        [InlineData("2022-01-01T12:00:00")]
        [InlineData("2022-01-01T00:00:00")]
        [InlineData("2022-01-20T23:59:59")]
        public void ShouldRegisterATimeMoment(string dateTimeStr)
        {
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);
            var hourPart = timeMoment.DateTime.ToLongTimeString();

            IEnumerable<TimeMoment> result = folhaDePonto.ClockIn(timeMoment);

            Assert.NotNull(result.FirstOrDefault(x => x.DateTime.ToLongTimeString() == hourPart));
        }


        [Theory]
        [InlineData("2022-01-01T12:00:00")]
        [InlineData("2022-01-01T00:00:00")]
        [InlineData("2022-01-01T23:00:00")]
        public void ShouldFailWhenRegisterATimeMomentThatAlreadyExists(string dateTimeStr)
        {
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);
            folhaDePonto.ClockIn(timeMoment);

            var exceptionCall = () => folhaDePonto.ClockIn(timeMoment);

            Assert.Throws<HourAlreadyExistsException>(exceptionCall);
        }

        [Theory]
        [InlineData("2022-01-01T12:00:00")]
        [InlineData("2022-01-01T00:00:00")]
        [InlineData("2022-01-01T16:59:59")]
        public void ShouldFailWhenExceedsTimeMomentRegister(string dateTimeStr)
        {
            TimeMoment timeMomentExtra = SeveralTimeMomentArranje(dateTimeStr, folhaDePonto);

            var exceptionCall = () => folhaDePonto.ClockIn(timeMomentExtra);

            Assert.Throws<HoursLimitExceptions>(exceptionCall);
        }

        #region Auxiliar methods
        private TimeMoment SeveralTimeMomentArranje(string dateTimeStr, IFolhaDePonto folhaDePonto)
        {
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);

            folhaDePonto.ClockIn(timeMoment);
            for (var i = 1; i < FolhaDePontoService.LIMIT_OF_MOMENT_PER_DAY; i++)
            {
                TimeMoment timeMomentRepeated = new TimeMoment { 
                    DateTime = timeMoment.DateTime.AddMinutes(i), 
                    UserId = timeMoment.UserId 
                };
                folhaDePonto.ClockIn(timeMomentRepeated);
            }

            TimeMoment timeMomentExtra = new TimeMoment { 
                DateTime = timeMoment.DateTime.AddHours(FolhaDePontoService.LIMIT_OF_MOMENT_PER_DAY), 
                UserId = timeMoment.UserId 
            };
            timeMomentExtra.UserId = timeMoment.UserId;
            return timeMomentExtra;
        }

        private static IFolhaDePonto FolhaDePontoServiceArranje()
        {
            Mock<ILogger<FolhaDePontoService>> mockLogger = new Mock<ILogger<FolhaDePontoService>>();
            SharedDatabaseFixture sharedDatabaseFixture = new SharedDatabaseFixture();
            var context = sharedDatabaseFixture.CreateContext();

            var timeMomentRepository = new TimeMomentRepository(context);
            IFolhaDePonto folhaDePonto = new FolhaDePontoService(mockLogger.Object, timeMomentRepository);
            return folhaDePonto;
        }

        private TimeMoment TimeMomentArranje(string dateTimeStr)
        {
            TimeMoment timeMoment = new TimeMoment();
            DateTime dateTime = DateTime.Parse(dateTimeStr);
            timeMoment.DateTime = dateTime;
            timeMoment.User = new User { Name = "teste" };
            return timeMoment;
        }
        #endregion
    }
}