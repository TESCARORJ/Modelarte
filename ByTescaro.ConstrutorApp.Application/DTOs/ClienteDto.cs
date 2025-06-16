using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs;

public class ClienteDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CpfCnpj { get; set; } = string.Empty;
    public TipoPessoa? TipoPessoa { get; set; }

    // Endereço
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;

    // Contato
    public string TelefonePrincipal { get; set; } = string.Empty;
    public string TelefoneWhatsApp { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string UsuarioCadastro { get; set; } = string.Empty;
    public DateTime DataHoraCadastro { get; set; }

    //Relacionamento
    public List<long> ProjetoIds { get; set; } = new();

}
