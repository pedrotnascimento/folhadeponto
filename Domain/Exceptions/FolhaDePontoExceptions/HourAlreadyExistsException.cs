namespace FolhaDePonto.Exceptions.FolhaDePontoExceptions
{
    public class HourAlreadyExistsException: Exception
    {

        HourAlreadyExistsException(): base("Horários já registrado") { 
        }
    }
}
