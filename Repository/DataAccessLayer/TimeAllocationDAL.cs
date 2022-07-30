namespace Repository.DataAccessLayer
{
    public class TimeAllocationDAL
    {
        public int UserId { get; set; }
        public UserDAL User { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeDuration { get; set; }
        public string ProjectName { get; set; }
    }
}
