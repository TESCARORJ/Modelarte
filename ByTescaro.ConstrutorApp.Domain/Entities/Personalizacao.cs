using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Personalizacao : EntidadeBase
    {
        public int Id { get; set; } = 1; // Usamos um ID fixo para garantir que só haja um registro

        // Empresa
        public string? NomeEmpresa { get; set; } 
        public string? LogotipoUrl { get; set; }
        public string? FaviconUrl { get; set; }

        // Contato
        public string? EnderecoEmpresa { get; set; }
        public string? TelefoneEmpresa { get; set; }
        public string? EmailEmpresa { get; set; }

        // Layout
        public string? TextoBoasVindas { get; set; }
        public string? TextoFooter { get; set; } 
        public string? ImagemFundoLoginUrl { get; set; }
        public string? CorHeader { get; set; } = "#0d6efd";
        public string? CorTextHeader { get; set; }
        public string? CorMenu { get; set; }

        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
