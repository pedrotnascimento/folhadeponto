namespace BusinessRule.Domain
{
    public class TimeAllocationBR
    {
        public int UserId { get; set; }
        public UserBR User { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeDuration { get; set; }
        public string ProjectName { get; set; }
    }
}
