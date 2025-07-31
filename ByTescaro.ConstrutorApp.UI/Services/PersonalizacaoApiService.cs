using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class PersonalizacaoApiService
    {
        private readonly HttpClient _http;

        // Cache simples para evitar múltiplas chamadas à API
        private PersonalizacaoDto? _cachedPersonalizacao;

        public PersonalizacaoApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PersonalizacaoDto?> ObterAsync()
        {
            if (_cachedPersonalizacao != null)
            {
                return _cachedPersonalizacao;
            }

            try
            {
                _cachedPersonalizacao = await _http.GetFromJsonAsync<PersonalizacaoDto>("api/personalizacao");
                return _cachedPersonalizacao;
            }
            catch (Exception ex)
            {
                // Log do erro ou notificação, dependendo da sua estratégia de tratamento de erros
                Console.WriteLine($"Erro ao obter personalização: {ex.Message}");
                // Retorna uma instância padrão em caso de falha
                return new PersonalizacaoDto { NomeEmpresa = "ConstrutorApp", TextoFooter = "&copy; 2025 ConstrutorApp" };
            }
        }

        public async Task AtualizarAsync(PersonalizacaoDto dto)
        {
            var response = await _http.PutAsJsonAsync("api/personalizacao", dto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao atualizar personalização: {response.StatusCode} - {errorContent}");
            }

            // Invalida o cache para que a próxima chamada à API busque os dados atualizados
            _cachedPersonalizacao = null;
        }
    }
}
