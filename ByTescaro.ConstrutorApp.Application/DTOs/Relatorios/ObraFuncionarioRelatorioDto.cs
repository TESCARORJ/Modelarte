using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraFuncionarioRelatorioDto
    {
        public string NomeFuncionario { get; set; }
        public string Funcao { get; set; }
        public DateTime DataInicioAlocacao { get; set; }
        public DateTime? DataFimAlocacao { get; set; }
        public decimal CustoDiario { get; set; }
    }
}
