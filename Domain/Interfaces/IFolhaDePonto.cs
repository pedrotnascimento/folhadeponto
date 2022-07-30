

using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IFolhaDePonto
    {
        IEnumerable<TimeMoment> ClockIn(TimeMoment dayMoment);
        TimeAllocation AllocateHoursInProject(TimeAllocation allocation);
    }
}
