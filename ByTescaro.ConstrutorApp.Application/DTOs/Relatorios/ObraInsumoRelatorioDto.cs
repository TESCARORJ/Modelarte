using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraInsumoRelatorioDto
    {
        public string? InsumoNome { get; set; } = string.Empty;
        public long? ResponsavelRecbimentoId { get; set; }
        public string? ResponsavelRecbimentoNome { get; set; } = string.Empty;
        public UnidadeMedida? UnidadeMedida { get; set; }
        public bool IsRecebido { get; set; } = false;
        public DateTime? DataRecebimento { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime DataHoraCadastro { get; set; }
    }
}
