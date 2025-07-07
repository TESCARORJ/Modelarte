using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IFornecedorService
    {
        Task<IEnumerable<FornecedorDto>> ObterTodosAsync();
        Task<FornecedorDto?> ObterPorIdAsync(long id);
        Task CriarAsync(FornecedorDto dto);
        Task AtualizarAsync(FornecedorDto dto);
        Task RemoverAsync(long id);

        /// <summary>
        /// Verifica se um cpf ou cnpj já existe para outro fornecedor.
        /// </summary>
        Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null);

        /// <summary>
        /// Verifica se um telefone principal já existe para outro fornecedor.
        /// </summary>
        Task<bool> TelefonePrincipalExistsAsync(string telefonePrincipal, long? ignoreId = null);

        /// <summary>
        /// Verifica se um WhatsApp já existe para outro fornecedor.
        /// </summary>
        Task<bool> TelefoneWhatsAppExistsAsync(string telefoneWhatsApp, long? ignoreId = null);

        /// <summary>
        /// Verifica se um email já existe para outro fornecedor.
        /// </summary>
        Task<bool> EmailExistsAsync(string email, long? ignoreId = null);

    }

}