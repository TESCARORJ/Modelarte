using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraEtapaRelatorioDto
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicioPrevista { get; set; }
        public DateTime? DataConclusaoPrevista { get; set; }
        public int PercentualConclusao { get; set; }
        public List<ObraItemEtapaRelatorioDto> Itens { get; set; } = new List<ObraItemEtapaRelatorioDto>();
    }
}
