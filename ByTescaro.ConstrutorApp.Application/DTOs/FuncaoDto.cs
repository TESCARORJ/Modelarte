namespace ByTescaro.ConstrutorApp.Application.DTOs;

public class FuncaoDto
{
    public long Id { get; set; }

    public string? Nome { get; set; } = string.Empty;
    public long? UsuarioCadastroId { get; set; }
    public string? UsuarioCadastroNome { get; set; } = string.Empty;
    public DateTime DataHoraCadastro { get; set; }


    /// <summary>
    /// Cria uma cópia superficial (shallow copy) da instância atual do FuncaoDto.
    /// Este método é útil para preservar o estado original do DTO em operações de edição.
    /// </summary>
    /// <returns>Uma nova instância de FuncaoDto com os mesmos valores de propriedade.</returns>
    public FuncaoDto Clone()
    {
        // MemberwiseClone é um método protegido, só pode ser chamado de dentro da própria classe ou derivado.
        // O tipo de retorno é object, então precisamos de um cast.
        return (FuncaoDto)this.MemberwiseClone();
    }
}
