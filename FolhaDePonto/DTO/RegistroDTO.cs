namespace FolhaDePonto.DTO
{
    public class RegistroDTO
    {
        public DateTime Dia { get; set; }
        public IEnumerable<string> Horarios { get; set; }
    }
}
