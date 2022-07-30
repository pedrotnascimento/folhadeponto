namespace BusinessRule.Domain
{
    public class TimeMomentBR
    {
        
        public int UserId { get; set; }
        public UserBR User { get; set; } 

        public DateTime DateTime { get; set; }
    }
}
