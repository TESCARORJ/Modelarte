using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraServicoDto
    {
        public long Id { get; set; }
        public long ObraServicoListaId { get; set; }
        public long ServicoId { get; set; }
        public string ServicoNome { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;

    }

}
