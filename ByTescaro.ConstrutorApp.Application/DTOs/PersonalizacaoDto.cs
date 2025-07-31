using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class PersonalizacaoDto
    {
        public string? NomeEmpresa { get; set; }
        public string? LogotipoUrl { get; set; }
        public string? FaviconUrl { get; set; }
        public string? EnderecoEmpresa { get; set; }
        public string? TelefoneEmpresa { get; set; }
        public string? EmailEmpresa { get; set; }
        public string? TextoBoasVindas { get; set; }
        public string? TextoFooter { get; set; }
        public string? ImagemFundoLoginUrl { get; set; }
        public string? CorHeader { get; set; }
        public string? CorTextHeader { get; set; }
        public string? CorDestaque { get; set; }
    }
}
