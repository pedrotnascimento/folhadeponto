using Domain.Entities;
using Domain.Interfaces;
using FolhaDePonto.Exceptions.FolhaDePontoExceptions;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.RepositoryInterfaces;

namespace Domain.Services
{
    public class FolhaDePontoService : IFolhaDePonto
    {
        public readonly static int LIMIT_OF_MOMENT_PER_DAY = 4;
        private ILogger<FolhaDePontoService> logger;
        private ITimeMomentRepository timeMomentRepository;
        private ITimeAllocationRepository timeAllocationRepository;

        public FolhaDePontoService(ILogger<FolhaDePontoService> _logger,
            ITimeMomentRepository timeMomentRepository,
            ITimeAllocationRepository timeAllocationRepository
            )
        {
            logger = _logger;
            this.timeMomentRepository = timeMomentRepository;
            this.timeAllocationRepository = timeAllocationRepository;
        }


        public IEnumerable<TimeMoment> ClockIn(TimeMoment dayMoment)
        {
            var timeMoments = timeMomentRepository.QueryByUserIdAndDate(dayMoment.UserId, dayMoment.DateTime.Date);
            var hasAlreadyTimeMomentInHour = timeMoments.Any(x => x.DateTime.Hour == dayMoment.DateTime.Hour &&
                    x.DateTime.Minute == dayMoment.DateTime.Minute);
            if (hasAlreadyTimeMomentInHour)
            {
                throw new HourAlreadyExistsException();
            }

            var hasExceedLimitOfMoments = timeMoments.Count() >= LIMIT_OF_MOMENT_PER_DAY;
            if (hasExceedLimitOfMoments)
            {
                throw new HoursLimitExceptions();
            }

            if (timeMoments.Count == 2)
            {
                TimeSpan lunchTime = dayMoment.DateTime - timeMoments.LastOrDefault().DateTime;
                if (lunchTime.Hours < 1)
                {
                    throw new LunchTimeLimitExceptions();
                }
            }

            if (dayMoment.DateTime.DayOfWeek == DayOfWeek.Sunday || dayMoment.DateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                throw new WeekendExceptions();
            }

            timeMomentRepository.Create(dayMoment);
            timeMoments.Add(dayMoment);
            return timeMoments;
        }

        public TimeAllocation AllocateHoursInProject(TimeAllocation allocation)
        {

            if (allocation.Date.DayOfWeek == DayOfWeek.Sunday || allocation.Date.DayOfWeek == DayOfWeek.Saturday)
            {
                throw new WeekendExceptions();
            }
            SanitizeTimeAllocation(allocation);
            ValidateAllocation(allocation);

            var timeAllocationOverall = timeAllocationRepository.GetByDate(allocation.Date);
            if (timeAllocationOverall != null)
            {
                timeAllocationRepository.Update(allocation);
            }
            else
            {
                timeAllocationRepository.Create(allocation);
            }

            return allocation;
        }

        private void ValidateAllocation(TimeAllocation allocation)
        {
            var hoursWorkedInDate = TimeWorkedInDate(allocation.UserId, allocation.Date);

            bool allocatingMoreThanTimeWorked = allocation.TimeDuration.Ticks > hoursWorkedInDate.Ticks;
            if (allocatingMoreThanTimeWorked)
            {
                throw new TimeAllocationLimitException();
            }
        }

        private static void SanitizeTimeAllocation(TimeAllocation allocation)
        {
            allocation.Date = allocation.Date.Date;
        }

        private DateTime TimeWorkedInDate(int userId, DateTime date)
        {
            double sum = 0;
            var result = timeMomentRepository.QueryByUserIdAndDate(userId, date);
            if (result == null)
                return DateTime.MinValue;

            if (result.Count >= 2)
            { 
                var dateTimeTillLunch  = result[1].DateTime.Subtract(result[0].DateTime);
                sum += dateTimeTillLunch.TotalHours;
            }

            if(result.Count == 4)
            {
                var dateTimeFromLunchTillEndOfTheDay  = result[2].DateTime.Subtract(result[3].DateTime);
                sum += dateTimeFromLunchTillEndOfTheDay.TotalHours;
            }

            var dateTime = DateTime.MinValue.AddHours(sum);
            return dateTime;
        }

    }
}
