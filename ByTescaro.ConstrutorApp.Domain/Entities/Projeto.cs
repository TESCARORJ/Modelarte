using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Projeto
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public StatusProjeto Status { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string TelefonePrincipal { get; set; } = string.Empty;


        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string UF { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;

        public long ClienteId { get; set; }
        public Cliente Cliente { get; set; } = default!;

        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;

        public ICollection<Obra> Obras { get; set; } = new List<Obra>();
    }
}