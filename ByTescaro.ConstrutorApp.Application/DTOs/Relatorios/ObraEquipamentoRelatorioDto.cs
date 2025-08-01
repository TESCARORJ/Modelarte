using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraEquipamentoRelatorioDto
    {
        public long ObraId { get; set; }     
        public long EquipamentoId { get; set; }
        public string? EquipamentoNome { get; set; } = string.Empty;
        public DateTime DataInicioUso { get; set; }
        public DateTime? DataFimUso { get; set; }
    }
}
