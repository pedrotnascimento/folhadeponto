using FolhaDePonto.DTO;

namespace FolhaDePonto.ViewModel
{
    public class ReportDTO
    {
        public string Mes { get; set; }
        public string HorasTrabalhadas { get; set; }
        public string HorasExcendentes { get; set; }
        public string HorasDevidas { get; set; }
        public IEnumerable<AllocationReportDTO> Alocacoes { get; set; }
        public IEnumerable<RegisterDTO> Registros { get; set; }
    }
}
