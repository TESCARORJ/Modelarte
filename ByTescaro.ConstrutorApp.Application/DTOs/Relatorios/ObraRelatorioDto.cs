using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraRelatorioDto
    {
        public long Id { get; set; }
        public string NomeObra { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicioObra { get; set; }
        public DateTime DataInicioProjeto { get; set; }
        public DateTime? DataConclusaoPrevista { get; set; }
        public int ProgressoAtual { get; set; }
        public string Status { get; set; }
        public string ClienteNome { get; set; }
        public string ProjetoNome { get; set; }
        public decimal OrcamentoTotal { get; set; }

        public List<ObraEtapaRelatorioDto> Etapas { get; set; } = new List<ObraEtapaRelatorioDto>();
        public List<ObraInsumoRelatorioDto> Insumos { get; set; } = new List<ObraInsumoRelatorioDto>();
        public List<ObraFuncionarioRelatorioDto> Funcionarios { get; set; } = new List<ObraFuncionarioRelatorioDto>();
        public List<ObraEquipamentoRelatorioDto> Equipamentos { get; set; } = new List<ObraEquipamentoRelatorioDto>();
        public List<ObraDocumentoRelatorioDto> Documentos { get; set; } = new List<ObraDocumentoRelatorioDto>();
        public List<ObraImagemRelatorioDto> Imagens { get; set; } = new List<ObraImagemRelatorioDto>();
        // Adicione outras propriedades conforme a necessidade do relatório
    }
}
