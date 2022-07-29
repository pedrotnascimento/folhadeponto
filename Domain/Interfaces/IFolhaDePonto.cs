

using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IFolhaDePonto
    {
        IEnumerable<TimeMoment> ClockIn(TimeMoment dayMoment);
        IEnumerable<TimeAllocation> AllocateHoursInProject(TimeAllocation allocation);
    }
}
