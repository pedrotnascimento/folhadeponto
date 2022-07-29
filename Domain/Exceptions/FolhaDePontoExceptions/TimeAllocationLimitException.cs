namespace FolhaDePonto.Exceptions.FolhaDePontoExceptions
{
    public class TimeAllocationLimitException : Exception
    {
        TimeAllocationLimitException(): base("Não pode alocar tempo maior que o tempo trabalhado no dia")
        {

        }
    }
}
