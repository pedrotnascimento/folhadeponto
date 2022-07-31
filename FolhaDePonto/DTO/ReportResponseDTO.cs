using FolhaDePonto.DTO;

namespace FolhaDePonto.DTO
{
    public class ReportResponseDTO
    {
        public string Mes { get; set; }
        public string HorasTrabalhadas { get; set; }
        public string HorasExcendentes { get; set; }
        public string HorasDevidas { get; set; }
        public IEnumerable<AllocationReportResponseDTO> Alocacoes { get; set; }
        public IEnumerable<RegisterResponseDTO> Registros { get; set; }
    }
}
