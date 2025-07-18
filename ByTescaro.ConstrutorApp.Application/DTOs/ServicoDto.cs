using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs;

public class ServicoDto
{
    public long Id { get; set; }
    public bool Ativo { get; set; }  
    public string? Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public long UsuarioCadastroId { get; set; }
    public string? UsuarioCadastroNome { get; set; } = string.Empty;
    public DateTime DataHoraCadastro { get; set; }

    /// <summary>
    /// Cria uma cópia superficial (shallow copy) da instância atual do ServicoDto.
    /// Este método é útil para preservar o estado original do DTO em operações de edição.
    /// </summary>
    /// <returns>Uma nova instância de ServicoDto com os mesmos valores de propriedade.</returns>
    public ServicoDto Clone()
    {
        // MemberwiseClone é um método protegido, só pode ser chamado de dentro da própria classe ou derivado.
        // O tipo de retorno é object, então precisamos de um cast.
        return (ServicoDto)this.MemberwiseClone();
    }

}
