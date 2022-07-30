using Moq;
using Microsoft.Extensions.Logging;
using FolhaDePonto.Exceptions.FolhaDePontoExceptions;
using Repository.Repositories;
using Repository;
using BusinessRule.Interfaces;
using BusinessRule.Exceptions.FolhaDePontoExceptions;
using BusinessRule.Services;
using BusinessRule.Domain;
using Repository.Tables;
using AutoMapper;
using FolhaDePonto.AutoMapper;

namespace FolhaDePontoTest
{
    public class FolhaDePontoServiceTest
    {
        private readonly DateTime defaultDateTime = new DateTime(2022,1,3,0,0,0);
        IFolhaDePonto folhaDePonto;
        User testUser;
        FolhaDePontoContext context;
        public FolhaDePontoServiceTest()
        {
            context = CreateContext();
            folhaDePonto = FolhaDePontoServiceArranje(context);
            CreateTestUser(context);
        }

        [Theory]
        [InlineData("2022-01-03T12:00:00")]
        [InlineData("2022-01-03T00:00:00")]
        [InlineData("2022-01-20T23:59:59")]
        public void ShouldRegisterATimeMoment(string dateTimeStr)
        {
            TimeMomentBR timeMoment = TimeMomentArranje(dateTimeStr);
            var hourPart = timeMoment.DateTime.ToLongTimeString();

            IEnumerable<TimeMomentBR> result = folhaDePonto.ClockIn(timeMoment);

            Assert.NotNull(result.FirstOrDefault(x => x.DateTime.ToLongTimeString() == hourPart));
        }


        [Theory]
        [InlineData("2022-01-03T09:00:00", "2022-01-03T12:00:00", "2022-01-03T13:00:00", "2022-01-03T17:00:00")]
        [InlineData("2022-01-03T00:00:00", "2022-01-03T00:01:00", "2022-01-03T23:58:00", "2022-01-03T23:59:00")]
        [InlineData("2022-01-03T00:00:00", "2022-01-03T22:58:00", "2022-01-03T23:58:00", "2022-01-03T23:59:00")]
        public void ShouldRegisterAllTimeMoments(string start, string lunchStart, string lunchEnd, string end)
        {
            TimeMomentBR startMoment = TimeMomentArranje(start);
            folhaDePonto.ClockIn(startMoment);
            int userId = startMoment.UserId;

            TimeMomentBR startLunchMoment = CreateMomenWithUser(lunchStart, userId);
            folhaDePonto.ClockIn(startLunchMoment);

            TimeMomentBR endLunchMoment = CreateMomenWithUser(lunchEnd, userId);
            folhaDePonto.ClockIn(endLunchMoment);

            TimeMomentBR endMoment = CreateMomenWithUser(end, userId);
            IEnumerable<TimeMomentBR>? result = folhaDePonto.ClockIn(endMoment);

            Assert.True(result.Count() == 4);
        }

        [Theory]
        [InlineData("2022-01-03T12:00:00")]
        [InlineData("2022-01-03T00:00:00")]
        [InlineData("2022-01-03T23:00:00")]
        public void ShouldFailWhenRegisterATimeMomentThatAlreadyExists(string dateTimeStr)
        {
            TimeMomentBR timeMoment = TimeMomentArranje(dateTimeStr);
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
            TimeMomentBR timeMomentExtra = SeveralTimeMomentArranje(dateTimeStr, folhaDePonto);

            var exceptionCall = () => folhaDePonto.ClockIn(timeMomentExtra);

            Assert.Throws<HoursLimitExceptions>(exceptionCall);
        }

        [Theory]
        [InlineData("2022-01-01T12:00:00")]
        [InlineData("2022-01-02T00:00:00")]
        public void ShouldFailWhenWeekend(string dateTimeStr)
        {
            TimeMomentBR timeMoment = TimeMomentArranje(dateTimeStr);

            var exceptionCall = () => folhaDePonto.ClockIn(timeMoment);

            Assert.Throws<WeekendExceptions>(exceptionCall);
        }

        [Theory]
        [InlineData("2022-01-03T08:00:00", "2022-01-03T12:00:00", "2022-01-03T12:01:00")]
        [InlineData("2022-01-03T13:11:00", "2022-01-03T13:12:00", "2022-01-03T14:11:00")]
        public void ShouldFailWhenLunchLessThan1Hour(string startJourney, string lunchStart, string lunchEnd)
        {

            TimeMomentBR startJourneyTimeMoment = TimeMomentArranje(startJourney);
            folhaDePonto.ClockIn(startJourneyTimeMoment);
            TimeMomentBR lunchStartTimeMoment = new TimeMomentBR
            {
                UserId = startJourneyTimeMoment.UserId,
                DateTime = DateTime.Parse(lunchStart)
            };
            folhaDePonto.ClockIn(lunchStartTimeMoment);

            TimeMomentBR lunchEndTimeMoment = new TimeMomentBR
            {
                UserId = lunchStartTimeMoment.UserId,
                DateTime = DateTime.Parse(lunchEnd)
            };
            var exceptionCall = () => folhaDePonto.ClockIn(lunchEndTimeMoment);

            Assert.Throws<LunchTimeLimitExceptions>(exceptionCall);
        }

        [Theory]
        [InlineData( 0.1, 4)]
        [InlineData( 4, 4)]
        [InlineData( 8, 8)]
        [InlineData( 20,20)]
        public void ShouldCreateAllocationHoursInProject(double hoursToAllocate, double hoursWorked)
        {
            BuildTimeMomentFullJourney(hoursWorked);

            var timeDuration = new DateTime(1, 1, 1, 0, 0, 0).AddHours(hoursToAllocate);
            var timeAllocation = new TimeAllocationBR
            {
                Date = defaultDateTime,
                TimeDuration = timeDuration,
                ProjectName = "Any Project Name",
                UserId = testUser.Id
            };
            var result = folhaDePonto.AllocateHoursInProject(timeAllocation);
            Assert.True(result.TimeDuration == timeDuration);
        }
       
        [Theory]
        [InlineData( 5, 4)]
        [InlineData( 4.1, 4)]
        [InlineData( 22,20)]
        public void ShouldFailWhenHoursToAllocateGreaterThanHoursWorked(double hoursToAllocate, double hoursWorked)
        {
            BuildTimeMomentFullJourney(hoursWorked);

            var timeDuration = new DateTime(1, 1, 1, 0, 0, 0).AddHours(hoursToAllocate);
            var timeAllocation = new TimeAllocationBR
            {
                Date = defaultDateTime,
                TimeDuration = timeDuration,
                ProjectName = "Any Project Name",
                UserId = testUser.Id
            };

            var exceptionCall = () => folhaDePonto.AllocateHoursInProject(timeAllocation);
            Assert.Throws<TimeAllocationLimitException>(exceptionCall);
        }
       

        #region Auxiliar methods

        private  FolhaDePontoContext CreateContext()
        {
            SharedDatabaseFixture sharedDatabaseFixture = new SharedDatabaseFixture();
            var context = sharedDatabaseFixture.CreateContext();
            return context;
        }

        private  void CreateTestUser(FolhaDePontoContext context)
        {
            testUser = new Repository.Tables.User { Name = "teste" };
            context.Users.Add(testUser);
            context.SaveChanges();
        }

        private TimeMomentBR CreateMomenWithUser(string dateTimeStr, int userId)
        {
            return new TimeMomentBR
            {
                UserId = userId,
                DateTime = DateTime.Parse(dateTimeStr),
            };
        }

        private TimeMomentBR SeveralTimeMomentArranje(string dateTimeStr, IFolhaDePonto folhaDePonto)
        {
            TimeMomentBR timeMoment = TimeMomentArranje(dateTimeStr);

            folhaDePonto.ClockIn(timeMoment);
            for (var i = 1; i < FolhaDePontoService.LIMIT_OF_MOMENT_PER_DAY; i++)
            {
                TimeMomentBR timeMomentRepeated = new TimeMomentBR
                {
                    DateTime = timeMoment.DateTime.AddHours(i),
                    UserId = timeMoment.UserId
                };
                folhaDePonto.ClockIn(timeMomentRepeated);
            }

            TimeMomentBR timeMomentExtra = new TimeMomentBR
            {
                DateTime = timeMoment.DateTime.AddHours(FolhaDePontoService.LIMIT_OF_MOMENT_PER_DAY),
                UserId = timeMoment.UserId
            };
            timeMomentExtra.UserId = timeMoment.UserId;
            return timeMomentExtra;
        }

        private static IFolhaDePonto FolhaDePontoServiceArranje(FolhaDePontoContext context)
        {
            Mock<ILogger<FolhaDePontoService>> mockLogger = new Mock<ILogger<FolhaDePontoService>>();
            var mapperConfiguration = new MapperConfiguration(cfg => {
                cfg.AddProfile(typeof(DTOtoBRProfileMapper));
                cfg.AddProfile(typeof(BRtoDALProfileMapper));
                cfg.AddProfile(typeof(DALtoTableProfileMapper));
            });

            var mapper = mapperConfiguration.CreateMapper();
            var timeMomentRepository = new TimeMomentRepository(context, mapper);
            var timeAllocationRepository= new TimeAllocationRepository(context, mapper);
            IFolhaDePonto folhaDePonto = new FolhaDePontoService(mockLogger.Object, mapper, timeMomentRepository, timeAllocationRepository);
            return folhaDePonto;
        }

        private TimeMomentBR TimeMomentArranje(string dateTimeStr)
        {
            TimeMomentBR timeMoment = new TimeMomentBR();
            DateTime dateTime = DateTime.Parse(dateTimeStr);
            timeMoment.DateTime = dateTime;
            timeMoment.UserId = this.testUser.Id;
            return timeMoment;
        }

        private void BuildTimeMomentFullJourney(double hoursWorked)
        {
            var list = new List<TimeMoment>();
            TimeMoment timeMomentStart = new TimeMoment { DateTime = defaultDateTime, UserId = testUser.Id };
            list.Add(timeMomentStart);
            list.Add(new TimeMoment { DateTime = defaultDateTime.AddHours(hoursWorked), UserId = testUser.Id });

            context.TimeMoments.AddRange(list);
            context.SaveChanges();
        }

        
        #endregion
    }
}