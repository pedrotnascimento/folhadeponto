using AutoMapper;
using BusinessRule.Domain;
using BusinessRule.Exceptions.FolhaDePontoExceptions;
using BusinessRule.Interfaces;
using FolhaDePonto.Exceptions.FolhaDePontoExceptions;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.DataAccessLayer;
using Repository.RepositoryInterfaces;

namespace BusinessRule.Services
{
    public class FolhaDePontoService : IFolhaDePonto
    {
        public readonly static int LIMIT_OF_MOMENT_PER_DAY = 4;
        private ILogger<FolhaDePontoService> logger;
        private IMapper mapper;
        private ITimeMomentRepository timeMomentRepository;
        private ITimeAllocationRepository timeAllocationRepository;

        public FolhaDePontoService(ILogger<FolhaDePontoService> _logger,
            IMapper mapper,
            ITimeMomentRepository timeMomentRepository,
            ITimeAllocationRepository timeAllocationRepository
            )
        {
            logger = _logger;
            this.mapper = mapper;
            this.timeMomentRepository = timeMomentRepository;
            this.timeAllocationRepository = timeAllocationRepository;
        }


        public IEnumerable<TimeMomentBR> ClockIn(TimeMomentBR dayMoment)
        {
            var timeMomentsResult = timeMomentRepository.QueryByUserIdAndDate(dayMoment.UserId, dayMoment.DateTime.Date);
            var timeMoments = mapper.Map<IList<TimeMomentDAL>, IList<TimeMomentBR>>(timeMomentsResult);

            var hasAlreadyTimeMomentInHour = timeMomentsResult.Any(x => x.DateTime.Hour == dayMoment.DateTime.Hour &&
                    x.DateTime.Minute == dayMoment.DateTime.Minute);
            

            ValidateClockIn(dayMoment, timeMoments, hasAlreadyTimeMomentInHour);
            
            var dayMomentForCreation = mapper.Map<TimeMomentBR, TimeMomentDAL>(dayMoment);
            
            timeMomentRepository.Create(dayMomentForCreation);
            timeMoments.Add(dayMoment);
            return timeMoments;
        }

        private void ValidateClockIn(TimeMomentBR dayMoment, IList<TimeMomentBR> timeMoments, bool hasAlreadyTimeMomentInHour)
        {
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
        }

        public TimeAllocationBR AllocateHoursInProject(TimeAllocationBR allocation)
        {

            SanitizeTimeAllocation(allocation);
            ValidateAllocation(allocation);

            var timeAllocationOverall = timeAllocationRepository.GetByDate(allocation.Date);
            var allocationForCreate = mapper.Map<TimeAllocationBR, TimeAllocationDAL>(allocation);
            if (timeAllocationOverall != null)
            {
                timeAllocationRepository.Update(allocationForCreate);
            }
            else
            {
                timeAllocationRepository.Create(allocationForCreate);
            }

            return allocation;
        }

        private void ValidateAllocation(TimeAllocationBR allocation)
        {
            var hoursWorkedInDate = TimeWorkedInDate(allocation.UserId, allocation.Date);

            bool allocatingMoreThanTimeWorked = allocation.TimeDuration.Ticks > hoursWorkedInDate.Ticks;
            if (allocatingMoreThanTimeWorked)
            {
                throw new TimeAllocationLimitException();
            }

            if (allocation.Date.DayOfWeek == DayOfWeek.Sunday || allocation.Date.DayOfWeek == DayOfWeek.Saturday)
            {
                throw new WeekendExceptions();
            }
        }

        private static void SanitizeTimeAllocation(TimeAllocationBR allocation)
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
                var dateTimeFromLunchTillEndOfTheDay  = result[3].DateTime.Subtract(result[2].DateTime);
                sum += dateTimeFromLunchTillEndOfTheDay.TotalHours;
            }

            var dateTime = DateTime.MinValue.AddHours(sum);
            return dateTime;
        }

        public TimeAllocationBR GetReport(ReportBR reportGetDTO)
        {
            throw new NotImplementedException();
        }
    }
}
