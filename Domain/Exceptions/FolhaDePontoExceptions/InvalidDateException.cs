namespace FolhaDePonto.Exceptions.FolhaDePontoExceptions
{
    public class InvalidDateException : Exception
    {
        InvalidDateException(): base("Data e hora em formato inválido")
        {

        }
    }
}
