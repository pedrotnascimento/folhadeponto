
namespace BusinessRule.Domain
{
    public class ReportDataBR
    {
        public DateTime Month { get; set; }
        public UserBR User { get; set; }
        public DateTime WorkedTime { get; set; }
        public DateTime ExceededWorkedTime { get; set; }
        public DateTime DebtTime { get; set; }
        public IEnumerable<TimeAllocationBR> TimeAllocations { get; set; }
        public IEnumerable<TimeMomentBR> TimeMoments { get; set; }
    }
}