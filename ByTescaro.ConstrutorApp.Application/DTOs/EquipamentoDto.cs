using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs;

public class EquipamentoDto
{
    public long Id { get; set; }
    public string? Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public string? Patrimonio { get; set; } = string.Empty;
    public StatusEquipamento Status { get; set; }
    public decimal? CustoLocacaoDiaria { get; set; }
    public long? UsuarioCadastroId { get; set; }
    public string? UsuarioCadastroNome { get; set; } = string.Empty;
    public DateTime DataHoraCadastro { get; set; }
    public string? ObraNome { get; set; } = string.Empty;
    public string? ProjetoNome { get; set; } = string.Empty;
    public string? ClienteNome { get; set; } = string.Empty;

    /// <summary>
    /// Cria uma cópia superficial (shallow copy) da instância atual do EquipamentoDto.
    /// Este método é útil para preservar o estado original do DTO em operações de edição.
    /// </summary>
    /// <returns>Uma nova instância de EquipamentoDto com os mesmos valores de propriedade.</returns>
    public EquipamentoDto Clone()
    {
        // MemberwiseClone é um método protegido, só pode ser chamado de dentro da própria classe ou derivado.
        // O tipo de retorno é object, então precisamos de um cast.
        return (EquipamentoDto)this.MemberwiseClone();
    }
}
