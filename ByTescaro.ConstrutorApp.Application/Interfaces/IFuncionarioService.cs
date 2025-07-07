using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces;

public interface IFuncionarioService
{
    Task<IEnumerable<FuncionarioDto>> ObterTodosAsync();
    Task<FuncionarioDto?> ObterPorIdAsync(long id);
    Task CriarAsync(FuncionarioDto dto);
    Task AtualizarAsync(FuncionarioDto dto);
    Task RemoverAsync(long id);

    //Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync();

    /// <summary>
    /// Verifica se um cpf ou cnpj já existe para outro funcionario.
    /// </summary>
    Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null);

    /// <summary>
    /// Verifica se um telefone principal já existe para outro funcionario.
    /// </summary>
    Task<bool> TelefonePrincipalExistsAsync(string telefonePrincipal, long? ignoreId = null);

    /// <summary>
    /// Verifica se um WhatsApp já existe para outro funcionario.
    /// </summary>
    Task<bool> TelefoneWhatsAppExistsAsync(string telefoneWhatsApp, long? ignoreId = null);

    /// <summary>
    /// Verifica se um email já existe para outro funcionario.
    /// </summary>
    Task<bool> EmailExistsAsync(string email, long? ignoreId = null);
}
