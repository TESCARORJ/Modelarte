using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraDocumentoRelatorioDto
    {
        public string NomeDocumento { get; set; }
        public string TipoArquivo { get; set; }
        public string CaminhoArquivo { get; set; } // Pode ser usado para linkar ou para log
    }
}
