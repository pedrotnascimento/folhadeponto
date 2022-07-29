namespace FolhaDePonto.Exceptions.FolhaDePontoExceptions
{
    public class LunchTimeLimitExceptions : Exception
    {
        public LunchTimeLimitExceptions() : base("Deve haver no mínimo 1 hora de almoço")
        {
        }
    }
}
