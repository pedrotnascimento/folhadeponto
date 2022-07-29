using AutoMapper;
using BusinessRule.Domain;
using BusinessRule.Exceptions.FolhaDePontoExceptions;
using BusinessRule.Interfaces;
using Common;
using FolhaDePonto.Exceptions.FolhaDePontoExceptions;
using Microsoft.Extensions.Logging;
using Repository.DataAccessLayer;
using Repository.RepositoryInterfaces;

namespace BusinessRule.Services
{
    public class FolhaDePontoService : IFolhaDePonto
    {
        public readonly static int LIMIT_OF_MOMENT_PER_DAY = 4;
        public readonly static int HOURS_PER_DAY = 8;
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

            var timeAllocationOverall = timeAllocationRepository.GetByUserIdAndDate(allocation.UserId, allocation.Date);
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
            IList<TimeMomentDAL>? dayMomentsDAL = timeMomentRepository.QueryByUserIdAndDate(userId, date);
            var dayMoments = mapper.Map< IList<TimeMomentDAL>, IList<TimeMomentBR>>(dayMomentsDAL);
            DateTime dateTime = TimeWorkedInDate(dayMoments);
            return dateTime;
        }

        private DateTime TimeWorkedInDate(IList<TimeMomentBR> timeMoments)
        {
            double sum = 0;
            if (timeMoments == null)
                return DateTime.MinValue;

            if (timeMoments.Count >= 2)
            {
                var dateTimeTillLunch = timeMoments[1].DateTime.Subtract(timeMoments[0].DateTime);
                sum += dateTimeTillLunch.TotalHours;
            }

            if (timeMoments.Count == 4)
            {
                var dateTimeFromLunchTillEndOfTheDay = timeMoments[3].DateTime.Subtract(timeMoments[2].DateTime);
                sum += dateTimeFromLunchTillEndOfTheDay.TotalHours;
            }

            DateTime dateTime = DateTime.MinValue.AddHours(sum);
            return dateTime;
        }

        public ReportDataBR? GetReport(ReportBR reportDTO)
        {
            DateTime month = reportDTO.Month;
            var result = new ReportDataBR
            {
                User = reportDTO.User,
                Month = month,
            };

            var timeMomentsDAL = timeMomentRepository.QueryByUserIdAndMonth(reportDTO.User.Id, month);
            if (!timeMomentsDAL.Any())
            {
                return null;
            }
            var timeMoments = mapper.Map<List<TimeMomentDAL>, List<TimeMomentBR>>(timeMomentsDAL);
            result.TimeMoments = timeMoments;

            DateTime totalWorkedTime = CalculateTotalWorkTime(timeMoments);
            result.WorkedTime = totalWorkedTime;

            CalculateCompensatoryTime(result);

            List<TimeAllocationDAL> timeAllocationsDAL = timeAllocationRepository.QueryByUserIdAndMonth(reportDTO.User.Id, month);
            var timeAllocations= mapper.Map<List<TimeAllocationDAL>, List<TimeAllocationBR>>(timeAllocationsDAL);
            result.TimeAllocations = timeAllocations;

            return result;
        }

        private void CalculateCompensatoryTime(ReportDataBR result)
        {
            var totalWorkedHours = new TimeSpan(result.WorkedTime.Ticks).TotalHours;
            int workHoursDemanded = DateHelper.HoursInAMonthCountedByFixedHours(result.Month, HOURS_PER_DAY);
            if (totalWorkedHours < workHoursDemanded)
            {
                var debtTime = workHoursDemanded - totalWorkedHours;
                result.DebtTime = DateTime.MinValue.AddHours(debtTime);
            }
            else
            {
                var exceedTime = totalWorkedHours - workHoursDemanded;
                result.ExceededWorkedTime = DateTime.MinValue.AddHours(exceedTime);
            }
        }

        private DateTime CalculateTotalWorkTime(List<TimeMomentBR> timeMoments)
        {
           
            var timeMomentsSorted = timeMoments.OrderBy(x => x.DateTime);
            TimeMomentBR? currentMoment = timeMomentsSorted.FirstOrDefault();
            var currentDay = currentMoment.DateTime.Day;
            var stackMoments = new List<TimeMomentBR>();
            stackMoments.Add(currentMoment);
            var totalTimeSum = DateTime.MinValue;
            for (int i =1; i < timeMoments.Count; i++)
            {
                
                var curr = timeMoments[i].DateTime;
                if(curr.Day== currentDay)
                {
                    stackMoments.Add(timeMoments[i]);
                    if (i == timeMoments.Count - 1)
                    {
                        var totalTimeInDay = TimeWorkedInDate(stackMoments);
                        totalTimeSum = totalTimeSum.AddTicks(totalTimeInDay.Ticks);
                    }
                }
                else
                {
                    var totalTimeInDay = TimeWorkedInDate(stackMoments);
                    totalTimeSum = totalTimeSum.AddTicks(totalTimeInDay.Ticks);
                    stackMoments.Clear();
                    stackMoments.Add(timeMoments[i]);
                    currentDay = timeMoments[i].DateTime.Day;
                }
            }
            return totalTimeSum;
        }
       
    }
}
