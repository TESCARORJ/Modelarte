using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraImagemRelatorioDto
    {
        public string NomeImagem { get; set; }
        public string CaminhoImagem { get; set; } // Pode ser usado para renderizar no PDF
    }
}
