namespace Repository.Tables
{
    public class TimeAllocation
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeDuration { get; set; }
        public string ProjectName { get; set; }
    }
}
