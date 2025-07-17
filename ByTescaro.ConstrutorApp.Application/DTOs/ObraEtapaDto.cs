using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraEtapaDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public string? Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }
        public StatusEtapa Status { get; set; }

        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string? UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }

        public List<ObraItemEtapaDto> Itens { get; set; } = new();

        public int Progresso =>
            Itens.Count == 0
            ? 0
            : (int)Math.Round((double)Itens.Count(i => i.Concluido) / Itens.Count * 100);
    }

}
