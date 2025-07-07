using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> ObterTodosAsync();
        Task<ClienteDto?> ObterPorIdAsync(long id);
        Task CriarAsync(ClienteDto dto);
        Task AtualizarAsync(ClienteDto dto);
        Task RemoverAsync(long id);

        /// <summary>
        /// Verifica se um cpf ou cnpj já existe para outro cliente.
        /// </summary>
        Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null);

        /// <summary>
        /// Verifica se um telefone principal já existe para outro cliente.
        /// </summary>
        Task<bool> TelefonePrincipalExistsAsync(string telefonePrincipal, long? ignoreId = null);

        /// <summary>
        /// Verifica se um WhatsApp já existe para outro cliente.
        /// </summary>
        Task<bool> TelefoneWhatsAppExistsAsync(string telefoneWhatsApp, long? ignoreId = null);

        /// <summary>
        /// Verifica se um email já existe para outro cliente.
        /// </summary>
        Task<bool> EmailExistsAsync(string email, long? ignoreId = null);

    }

}