using FolhaDePonto.DTO;

namespace FolhaDePonto.ViewModel
{
    public class RelatorioDTO
    {
        public string Mes { get; set; }
        public string HorasTrabalhadas { get; set; }
        public string HorasExcendentes { get; set; }
        public string HorasDevidas { get; set; }
        public IEnumerable<AlocacaoDTO> Alocacoes { get; set; }
        public IEnumerable<RegistroDTO> Registros { get; set; }
    }
}
