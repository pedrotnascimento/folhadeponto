namespace Domain.Entities
{
    public class TimeAllocation
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeDuration { get; set; }
        public string ProjectName { get; set; }
    }
}
