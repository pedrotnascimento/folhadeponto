using FolhaDePonto.Entities;
using FolhaDePonto.Interfaces;
using Microsoft.Extensions.Logging;

namespace FolhaDePonto.Services
{
    public class FolhaDePontoService : IFolhaDePonto
    {
        private ILogger<FolhaDePontoService> logger;

        public FolhaDePontoService(ILogger<FolhaDePontoService> _logger)
        {
            logger = _logger;
        }

        public IEnumerable<TimeAllocation> AllocateHoursInProject(TimeAllocation allocation)
        {
            throw new NotImplementedException();
        }

        public Register ClockIn(TimeMoment dayMoment)
        {
            throw new NotImplementedException();
        }
    }
}
