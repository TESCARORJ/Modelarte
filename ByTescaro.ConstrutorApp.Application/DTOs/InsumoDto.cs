using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs;

public class InsumoDto
{
    public long Id { get; set; }
    public bool Ativo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public UnidadeMedida? UnidadeMedida { get; set; }
    public string UsuarioCadastro { get; set; } = string.Empty;
    public DateTime DataHoraCadastro { get; set; }

}
