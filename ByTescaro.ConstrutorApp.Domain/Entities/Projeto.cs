using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Projeto : EntidadeBase
    {
        public string? Nome { get; set; } = string.Empty;
        public StatusProjeto Status { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? TelefonePrincipal { get; set; } = string.Empty;
        public long? EnderecoId { get; set; }
        public Endereco Endereco { get; set; } 
        public long? ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; } 
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
        public ICollection<Obra> Obras { get; set; } = new List<Obra>();
    }
}