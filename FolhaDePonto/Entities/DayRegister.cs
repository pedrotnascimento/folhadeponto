namespace FolhaDePonto.Entities
{
    public class DayRegister
    {
        public DateTime Date { get; set; }
        public IEnumerable<DateTime> Hours { get; set; }
    }
}
