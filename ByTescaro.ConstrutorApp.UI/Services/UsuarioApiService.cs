using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class UsuarioApiService
    {
        private readonly HttpClient _http;

        public UsuarioApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<UsuarioDto>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/usuario");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao buscar usuários: {response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<List<UsuarioDto>>() ?? new();
        }

        public async Task<UsuarioDto?> GetByIdAsync(long id)
        {
            var response = await _http.GetAsync($"api/usuario/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UsuarioDto>();
        }

        public async Task CreateAsync(UsuarioDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/usuario", dto);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao criar usuário: {response.StatusCode}");
        }

        public async Task UpdateAsync(UsuarioDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/usuario/{dto.Id}", dto);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = $"Erro ao atualizar usuário: {response.StatusCode}";

                try
                {
                    // Tenta ler o conteúdo da resposta se houver um erro,
                    // que geralmente contém detalhes adicionais da API.
                    var errorContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(errorContent))
                    {
                        errorMessage += $" - Detalhes: {errorContent}";
                    }
                }
                catch (Exception ex)
                {
                    // Em caso de falha ao ler o conteúdo do erro (raro, mas possível),
                    // registra a exceção para depuração.
                    Console.WriteLine($"Erro ao ler o conteúdo da resposta de erro: {ex.Message}");
                }

                throw new Exception(errorMessage);
            }
        }

        public async Task DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"api/usuario/{id}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao excluir usuário: {response.StatusCode}");
        }

        public async Task InativarAsync(long id)
        {
            var response = await _http.PostAsync($"api/usuario/inativar/{id}", null);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao inativar usuário: {response.StatusCode}");
        }
    }
}