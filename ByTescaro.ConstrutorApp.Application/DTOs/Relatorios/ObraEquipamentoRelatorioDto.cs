using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraEquipamentoRelatorioDto
    {
        public string NomeEquipamento { get; set; }
        public string Status { get; set; }
        public DateTime DataInicioAlocacao { get; set; }
        public DateTime? DataFimAlocacao { get; set; }
        public decimal CustoDiario { get; set; }
    }
}
