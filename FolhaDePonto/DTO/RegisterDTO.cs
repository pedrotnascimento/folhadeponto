namespace FolhaDePonto.DTO
{
    public class RegisterDTO
    {
        public DateTime Dia { get; set; }
        public IEnumerable<string> Horarios { get; set; }
    }
}
