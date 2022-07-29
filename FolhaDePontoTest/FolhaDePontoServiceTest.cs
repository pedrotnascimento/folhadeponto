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
        [InlineData("2022-01-03T12:00:00")]
        [InlineData("2022-01-03T00:00:00")]
        [InlineData("2022-01-20T23:59:59")]
        public void ShouldRegisterATimeMoment(string dateTimeStr)
        {
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);
            var hourPart = timeMoment.DateTime.ToLongTimeString();

            IEnumerable<TimeMoment> result = folhaDePonto.ClockIn(timeMoment);

            Assert.NotNull(result.FirstOrDefault(x => x.DateTime.ToLongTimeString() == hourPart));
        }


        [Theory]
        [InlineData("2022-01-03T09:00:00", "2022-01-03T12:00:00", "2022-01-03T13:00:00", "2022-01-03T17:00:00")]
        [InlineData("2022-01-03T00:00:00", "2022-01-03T00:01:00", "2022-01-03T23:58:00", "2022-01-03T23:59:00")]
        [InlineData("2022-01-03T00:00:00", "2022-01-03T22:58:00", "2022-01-03T23:58:00", "2022-01-03T23:59:00")]
        public void ShouldRegisterAllTimeMoments(string start, string lunchStart, string lunchEnd, string end )
        {
            TimeMoment startMoment = TimeMomentArranje(start);
            folhaDePonto.ClockIn(startMoment);
            int userId = startMoment.UserId;

            TimeMoment startLunchMoment = CreateMomenWithUser(lunchStart, userId);
            folhaDePonto.ClockIn(startLunchMoment);

            TimeMoment endLunchMoment = CreateMomenWithUser(lunchEnd, userId);
            folhaDePonto.ClockIn(endLunchMoment);

            TimeMoment endMoment = CreateMomenWithUser(end, userId);
            IEnumerable<TimeMoment>? result = folhaDePonto.ClockIn(endMoment);

            Assert.True(result.Count()==4);
        }

        private  TimeMoment CreateMomenWithUser(string dateTimeStr, int userId)
        {
            return new TimeMoment
            {
                UserId = userId,
                DateTime = DateTime.Parse(dateTimeStr),
            };
        }

        [Theory]
        [InlineData("2022-01-03T12:00:00")]
        [InlineData("2022-01-03T00:00:00")]
        [InlineData("2022-01-03T23:00:00")]
        public void ShouldFailWhenRegisterATimeMomentThatAlreadyExists(string dateTimeStr)
        {
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);
            folhaDePonto.ClockIn(timeMoment);

            var exceptionCall = () => folhaDePonto.ClockIn(timeMoment);

            Assert.Throws<HourAlreadyExistsException>(exceptionCall);
        }

        [Theory]
        [InlineData("2022-01-03T12:00:00")]
        [InlineData("2022-01-03T00:00:00")]
        [InlineData("2022-01-03T16:59:59")]
        public void ShouldFailWhenExceedsTimeMomentRegister(string dateTimeStr)
        {
            TimeMoment timeMomentExtra = SeveralTimeMomentArranje(dateTimeStr, folhaDePonto);

            var exceptionCall = () => folhaDePonto.ClockIn(timeMomentExtra);

            Assert.Throws<HoursLimitExceptions>(exceptionCall);
        }

        [Theory]
        [InlineData("2022-01-01T12:00:00")]
        [InlineData("2022-01-02T00:00:00")]
        public void ShouldFailWhenWeekend(string dateTimeStr)
        {
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);

            var exceptionCall = () => folhaDePonto.ClockIn(timeMoment);

            Assert.Throws<WeekendExceptions>(exceptionCall);
        }

        [Theory]
        [InlineData("2022-01-03T08:00:00", "2022-01-03T12:00:00", "2022-01-03T12:01:00")]
        [InlineData("2022-01-03T13:11:00", "2022-01-03T13:12:00", "2022-01-03T14:11:00")]
        public void ShouldFailWhenLunchLessThan1Hour(string startJourney, string lunchStart, string lunchEnd)
        {

            TimeMoment startJourneyTimeMoment = TimeMomentArranje(startJourney);
            folhaDePonto.ClockIn(startJourneyTimeMoment);
            TimeMoment lunchStartTimeMoment = new TimeMoment
            {
                UserId = startJourneyTimeMoment.UserId,
                DateTime = DateTime.Parse(lunchStart)
            };
            folhaDePonto.ClockIn(lunchStartTimeMoment);

            TimeMoment lunchEndTimeMoment = new TimeMoment { 
                UserId = lunchStartTimeMoment.UserId,
                DateTime = DateTime.Parse(lunchEnd) 
            };
            var exceptionCall = () => folhaDePonto.ClockIn(lunchEndTimeMoment);

            Assert.Throws<LunchTimeLimitExceptions>(exceptionCall);
        }

        #region Auxiliar methods
        private TimeMoment SeveralTimeMomentArranje(string dateTimeStr, IFolhaDePonto folhaDePonto)
        {
            TimeMoment timeMoment = TimeMomentArranje(dateTimeStr);

            folhaDePonto.ClockIn(timeMoment);
            for (var i = 1; i < FolhaDePontoService.LIMIT_OF_MOMENT_PER_DAY; i++)
            {
                TimeMoment timeMomentRepeated = new TimeMoment { 
                    DateTime = timeMoment.DateTime.AddHours(i), 
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