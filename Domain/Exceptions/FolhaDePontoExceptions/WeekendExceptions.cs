namespace FolhaDePonto.Exceptions.FolhaDePontoExceptions
{
    public class WeekendExceptions : Exception
    {
        public WeekendExceptions() : base("Sábado e domingo não são permitidos como dia de trabalho")
        {
        }
    }
}
