using System.Reflection;
using System.Text.Json;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;


namespace ByTescaro.ConstrutorApp.Application.Services
{

    public class AuditoriaService : IAuditoriaService
    {
        private readonly ILogAuditoriaRepository _logRepository;

        public AuditoriaService(ILogAuditoriaRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task RegistrarCriacaoAsync<T>(T entidadeNova, string usuario) where T : class
        {
            var log = CriarLog(entidadeNova, null, usuario, "Criado");
            await _logRepository.RegistrarAsync(log);
        }

        public async Task RegistrarAtualizacaoAsync<T>(
            T entidadeAntiga,
            T entidadeNova,
            string usuario,
            Dictionary<string, Dictionary<long, string>>? colecoesNomes = null) where T : class
        {
            var log = CriarLog(entidadeNova, entidadeAntiga, usuario, "Atualizado");
            log.Descricao = GerarDescricaoDiferencas(entidadeAntiga, entidadeNova, colecoesNomes);
            await _logRepository.RegistrarAsync(log);
        }

        public async Task RegistrarExclusaoAsync<T>(T entidadeAntiga, string usuario) where T : class
        {
            var log = CriarLog(null, entidadeAntiga, usuario, "Excluído");
            await _logRepository.RegistrarAsync(log);
        }

        private LogAuditoria CriarLog<T>(T? dadosNovos, T? dadosAntigos, string usuario, string acao) where T : class
        {
            var entidade = typeof(T).Name;
            var entidadeFonte = dadosNovos ?? dadosAntigos!;
            string idEntidade = ObterIdEntidade(entidadeFonte);

            return new LogAuditoria
            {
                Usuario = usuario,
                Entidade = entidade,
                IdEntidade = idEntidade,
                Acao = acao,
                Descricao = $"{entidade} foi {acao.ToLower()} por {usuario}",
                DataHora = DateTime.Now,
                DadosAtuais = dadosNovos != null
                    ? JsonSerializer.Serialize(dadosNovos, new JsonSerializerOptions { WriteIndented = true })
                    : null,
                DadosAnteriores = dadosAntigos != null
                    ? JsonSerializer.Serialize(dadosAntigos, new JsonSerializerOptions { WriteIndented = true })
                    : null
            };
        }

        private string ObterIdEntidade<T>(T entidade) where T : class
        {
            var tipo = entidade.GetType();
            var propId = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p =>
                    string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(p.Name, "IdEntidade", StringComparison.OrdinalIgnoreCase));

            var valor = propId?.GetValue(entidade);
            if (valor == null)
                throw new InvalidOperationException($"A entidade '{tipo.Name}' não possui valor na propriedade 'Id'.");

            return valor.ToString()!;
        }

        private string GerarDescricaoDiferencas<T>(
            T antigo,
            T novo,
            Dictionary<string, Dictionary<long, string>>? colecoesNomes = null) where T : class
        {
            var tipo = typeof(T);
            var props = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var alteracoes = new List<string>();

            foreach (var prop in props)
            {
                var nomeProp = prop.Name;
                var tipoProp = prop.PropertyType;
                var valorAntigo = prop.GetValue(antigo);
                var valorNovo = prop.GetValue(novo);

                // Comparar coleções simples
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(tipoProp) && tipoProp != typeof(string))
                {
                    var itemType = tipoProp.IsGenericType ? tipoProp.GetGenericArguments()[0] : null;

                    if (itemType != null && (itemType == typeof(long) || itemType == typeof(int)))
                    {
                        var listaAntiga = (valorAntigo as System.Collections.IEnumerable)?
                            .Cast<object>()
                            .Select(x => x?.ToString())
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .ToList() ?? new();

                        var listaNova = (valorNovo as System.Collections.IEnumerable)?
                            .Cast<object>()
                            .Select(x => x?.ToString())
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .ToList() ?? new();

                        var removidos = listaAntiga.Except(listaNova).Select(x => long.TryParse(x, out var id) ? id : 0).Where(id => id > 0).ToList();
                        var adicionados = listaNova.Except(listaAntiga).Select(x => long.TryParse(x, out var id) ? id : 0).Where(id => id > 0).ToList();


                        if (removidos.Any() || adicionados.Any())
                        {
                            var nomesDisponiveis = colecoesNomes?.ContainsKey(nomeProp) == true
                                ? colecoesNomes[nomeProp]
                                : new Dictionary<long, string>();

                            string TraduzirIds(IEnumerable<long> ids) =>
                                string.Join(", ", ids.Select(id => nomesDisponiveis.TryGetValue(id, out var nome) ? nome : $"ID {id}"));

                            var msg = $"Campo \"{nomeProp}\"";

                            if (removidos.Any())
                                msg += $" - Removidos: [{TraduzirIds(removidos)}]";

                            if (adicionados.Any())
                                msg += $" - Adicionados: [{TraduzirIds(adicionados)}]";

                            alteracoes.Add(msg);
                        }
                    }

                    continue;
                }

                // Propriedades simples
                if (tipoProp.IsClass && tipoProp != typeof(string))
                    continue;

                string strAntigo = valorAntigo != null ? valorAntigo.ToString()?.Trim() ?? "" : "";
                string strNovo = valorNovo != null ? valorNovo.ToString()?.Trim() ?? "" : "";


                if (strAntigo != strNovo)
                {
                    alteracoes.Add($"Campo \"{nomeProp}\" alterado de \"{strAntigo}\" para \"{strNovo}\"");
                }
            }

            return alteracoes.Count > 0
                ? string.Join("; ", alteracoes)
                : "Nenhuma alteração significativa detectada.";
        }
    }

}