namespace Repository.Tables
{
    public class TimeMoment
    {
        
        public int UserId { get; set; }
        public User User { get; set; } 

        public DateTime DateTime { get; set; }
    }
}
