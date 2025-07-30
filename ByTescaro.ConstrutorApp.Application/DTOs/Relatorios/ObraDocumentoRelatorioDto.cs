using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraDocumentoRelatorioDto
    {
        public long ObraId { get; set; }
        public Obra Obra { get; set; }
        public string? NomeOriginal { get; set; } = string.Empty;
        public string? CaminhoRelativo { get; set; } = string.Empty;
        public string? Extensao { get; set; } = string.Empty;
        public long TamanhoEmKb { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
