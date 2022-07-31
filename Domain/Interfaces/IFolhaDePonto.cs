

using BusinessRule.Domain;

namespace BusinessRule.Interfaces
{
    public interface IFolhaDePonto
    {
        IEnumerable<TimeMomentBR> ClockIn(TimeMomentBR dayMoment);
        TimeAllocationBR AllocateHoursInProject(TimeAllocationBR allocation);
        ReportDataBR GetReport(ReportBR reportGetDTO);
    }
}
