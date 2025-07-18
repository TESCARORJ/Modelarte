using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs;

public class ClienteDto
{
    public long Id { get; set; }
    public string? Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; } = string.Empty;
    public TipoPessoa? TipoPessoa { get; set; }

    // Endereço
    public string? Logradouro { get; set; } = string.Empty;
    public string? Numero { get; set; } = string.Empty;
    public string? Bairro { get; set; } = string.Empty;
    public string? Cidade { get; set; } = string.Empty;
    public string? Estado { get; set; } = string.Empty;
    public string? UF { get; set; } = string.Empty;
    public string? CEP { get; set; } = string.Empty;
    public string? Complemento { get; set; } = string.Empty;

    // Contato
    public string? TelefonePrincipal { get; set; } = string.Empty;
    public string? TelefoneWhatsApp { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;

    public long UsuarioCadastroId { get; set; }
    public string? UsuarioCadastroNome { get; set; } = string.Empty;
    public DateTime DataHoraCadastro { get; set; }

    //Relacionamento
    public List<long> ProjetoIds { get; set; } = new();

    /// <summary>
    /// Cria uma cópia superficial (shallow copy) da instância atual do ClienteDto.
    /// Este método é útil para preservar o estado original do DTO em operações de edição.
    /// </summary>
    /// <returns>Uma nova instância de ClienteDto com os mesmos valores de propriedade.</returns>
    public ClienteDto Clone()
    {
        // MemberwiseClone é um método protegido, só pode ser chamado de dentro da própria classe ou derivado.
        // O tipo de retorno é object, então precisamos de um cast.
        return (ClienteDto)this.MemberwiseClone();
    }

}
