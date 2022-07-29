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
        private readonly FolhaDePontoContext context;

        public FolhaDePontoService(ILogger<FolhaDePontoService> _logger,
            ITimeMomentRepository timeMomentRepository)
        {
            this.context = context;
            logger = _logger;
            this.timeMomentRepository = timeMomentRepository;
        }

        public IEnumerable<TimeAllocation> AllocateHoursInProject(TimeAllocation allocation)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TimeMoment> ClockIn(TimeMoment dayMoment)
        {
            var timeMoments = timeMomentRepository.QueryByUserIdAndDate(dayMoment.UserId, dayMoment.DateTime.Date);
            var hasAlreadyTimeMomentInHour = timeMoments.Any(x => x.DateTime.Hour == dayMoment.DateTime.Hour);
            if (hasAlreadyTimeMomentInHour)
            {
                throw new HourAlreadyExistsException();
            }

            var hasExceedLimitOfMoments = timeMoments.Count() >= LIMIT_OF_MOMENT_PER_DAY;
            if (hasExceedLimitOfMoments)
            {
                throw new HoursLimitExceptions();
            }

            timeMomentRepository.Create(dayMoment);
            timeMoments.Add(dayMoment);
            return timeMoments;
        }
    }
}
