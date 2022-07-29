namespace FolhaDePonto.Exceptions.FolhaDePontoExceptions
{
    public class HourAlreadyExistsException: Exception
    {

        public HourAlreadyExistsException(): base("Horários já registrado") { 
        }
    }
}
