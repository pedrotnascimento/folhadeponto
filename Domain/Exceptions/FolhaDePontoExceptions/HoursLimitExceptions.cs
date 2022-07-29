namespace FolhaDePonto.Exceptions.FolhaDePontoExceptions
{
    public class HoursLimitExceptions : Exception
    {
        public HoursLimitExceptions() : base("Apenas 4 horários podem ser registrados por dia")
        {
        }
    }
}
