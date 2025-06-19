using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraItemEtapaPadraoDto
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public long ObraEtapaId { get; set; }
        public string ObraEtapaNome { get; set; } = string.Empty;

        public int Ordem { get; set; }

        public bool Concluido { get; set; }
        public bool IsDataPrazo { get; set; }
        public int? DiasPrazo { get; set; }
        public bool PrazoAtivo { get; set; }
        public DateTime? DataConclusao { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;

        public List<ObraItemEtapaPadraoInsumoDto> Insumos { get; set; } = new();

    }

}
