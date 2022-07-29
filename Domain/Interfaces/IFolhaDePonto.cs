using FolhaDePonto.Entities;

namespace FolhaDePonto.Interfaces
{
    public interface IFolhaDePonto
    {
        IEnumerable<Register> ClockIn(TimeMoment dayMoment);
        IEnumerable<TimeAllocation> AllocateHoursInProject(TimeAllocation allocation);
    }
}
