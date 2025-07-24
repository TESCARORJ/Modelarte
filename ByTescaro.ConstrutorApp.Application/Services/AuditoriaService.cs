using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Application.Utils;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ByTescaro.ConstrutorApp.Application.Services
{

    public class AuditoriaService : IAuditoriaService
    {
        private readonly ILogAuditoriaRepository _logRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public AuditoriaService(ILogAuditoriaRepository logRepository, IUsuarioRepository usuarioRepository)
        {
            _logRepository = logRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task RegistrarCriacaoAsync<T>(T entidadeNova, long usuarioId) where T : class
        {
            var usuario = _usuarioRepository.GetByIdAsync(usuarioId).Result;
            var usuarioNome = usuario != null ? usuario.Nome : string.Empty;

            var entidade = typeof(T).Name;
            var idProp = entidadeNova.GetType().GetProperty("Id");
            var nomeProp = entidadeNova.GetType().GetProperty("Nome");
            var descricaoProp = entidadeNova.GetType().GetProperty("Descricao");

            var idValor = idProp != null ? idProp.GetValue(entidadeNova) : null;
            var nomeValor = nomeProp != null ? nomeProp.GetValue(entidadeNova) : null;
            var descricaoValor = descricaoProp != null ? descricaoProp.GetValue(entidadeNova) : null;

            var log = CriarLog(entidadeNova, null, usuarioId, TipoLogAuditoria.Criacao);

            if (entidade == "ObraPendencia")
            {
                log.Descricao = $"{descricaoValor} de Id {idValor} foi {EnumHelper.ObterDescricaoEnum(TipoLogAuditoria.Criacao)} por {usuarioNome} em {DateTime.Now}";

            }
            else
            {
                log.Descricao = $"{nomeValor} de Id {idValor} foi {EnumHelper.ObterDescricaoEnum(TipoLogAuditoria.Criacao)} por {usuarioNome} em {DateTime.Now}";

            }

            await _logRepository.RegistrarAsync(log);
        }

        public async Task RegistrarAtualizacaoAsync<T>(
     T entidadeAntiga,
     T entidadeNova,
     long usuarioId,
     Dictionary<string, Dictionary<long, string>>? colecoesNomes = null) where T : class
        {
            // A logica atual de GerarDescricaoDiferencas retorna uma string, nao uma lista de strings.
            // Você precisaria adaptar GerarDescricaoDiferencas para retornar uma lista de LogAuditoria ou uma lista de descrições individuais.

            // Opção 1: GerarDescricaoDiferencas retorna List<string> com as mensagens individuais
            var alteracoesIndividuais = GerarDescricaoDiferencasDetalhadas(entidadeAntiga, entidadeNova, colecoesNomes); // Novo método

            if (!alteracoesIndividuais.Any())
            {
                // Se não houver alterações significativas, não registra log ou registra um log de "nenhuma alteração".
                var log = CriarLog(entidadeNova, entidadeAntiga, usuarioId, TipoLogAuditoria.Atualizacao);
                log.Descricao = "Nenhuma alteração significativa detectada.";
                await _logRepository.RegistrarAsync(log);
                return;
            }

            foreach (var descricaoAlteracao in alteracoesIndividuais)
            {
                // Cria um novo log para CADA alteração de atributo
                var log = CriarLog(entidadeNova, entidadeAntiga, usuarioId, TipoLogAuditoria.Atualizacao);
                log.Descricao = descricaoAlteracao; // A descrição será apenas a mudança de um atributo
                await _logRepository.RegistrarAsync(log); // Persiste CADA log individualmente
            }
        }

        public async Task RegistrarExclusaoAsync<T>(T entidadeAntiga, long usuarioId) where T : class
        {
            var log = CriarLog(null, entidadeAntiga, usuarioId, TipoLogAuditoria.Exclusao);
            await _logRepository.RegistrarAsync(log);
        }

        private LogAuditoria CriarLog<T>(T? dadosNovos, T? dadosAntigos, long usuarioId, TipoLogAuditoria acao) where T : class
        {
            var entidade = typeof(T).Name;
            var entidadeFonte = dadosNovos ?? dadosAntigos!;
            string idEntidade = ObterIdEntidade(entidadeFonte);
            var usuario = _usuarioRepository.GetByIdAsync(usuarioId).Result;
            var usuarioNome = usuario != null ? usuario.Nome : string.Empty;

            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                // Esta opção resolve o erro de ciclo de referência
                ReferenceHandler = ReferenceHandler.Preserve
            };

            return new LogAuditoria
            {
                UsuarioId = usuarioId,
                UsuarioNome = usuarioNome,
                Entidade = entidade,
                IdEntidade = idEntidade,
                TipoLogAuditoria = acao,
                Descricao = $"{entidade} foi {EnumHelper.ObterDescricaoEnum(acao)} por {usuarioNome} em {DateTime.Now}",
                DataHora = DateTime.Now,
                DadosAtuais = dadosNovos != null ? JsonSerializer.Serialize(dadosNovos, serializerOptions) : null,
                DadosAnteriores = dadosAntigos != null ? JsonSerializer.Serialize(dadosAntigos, serializerOptions) : null
            };
        }

        private string ObterIdEntidade<T>(T entidade) where T : class
        {
            var tipo = entidade.GetType();

            return tipo.Name.ToString()!;
        }

        private List<string> GerarDescricaoDiferencasDetalhadas<T>(
    T antigo,
    T novo,
    Dictionary<string, Dictionary<long, string>>? colecoesNomes = null) where T : class
        {



            var tipo = typeof(T);
            var propriedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var alteracoes = new List<string>();

            foreach (var prop in propriedades)
            {
                var nomeProp = prop.Name;
                var tipoProp = prop.PropertyType;
                var valorAntigo = prop.GetValue(antigo);
                var valorNovo = prop.GetValue(novo);

                // 1. Lidar com coleções de tipos primitivos (long/int)
                if (typeof(IEnumerable).IsAssignableFrom(tipoProp) && tipoProp != typeof(string))
                {
                    var tipoElemento = tipoProp.IsGenericType ? tipoProp.GetGenericArguments()[0] : null;

                    // Apenas se for uma coleção de long ou int
                    if (tipoElemento == typeof(long) || tipoElemento == typeof(int))
                    {
                        CompararColecaoDeNumeros(nomeProp, valorAntigo as IEnumerable, valorNovo as IEnumerable, colecoesNomes, alteracoes);
                        continue; // Já tratou a propriedade, vai para a próxima.
                    }
                    // Se for uma coleção de outros tipos, você pode adicionar lógica aqui
                    // ou simplesmente ignorar se não for relevante para sua necessidade.
                    continue;
                }

                // 2. Lidar com propriedades de tipos complexos (outras classes)
                // Se a propriedade é uma classe e não é uma string, não é um tipo primitivo.
                if (tipoProp.IsClass && tipoProp != typeof(string))
                {
                    // **Importante:** Para comparar objetos aninhados, você precisaria de recursão.
                    // Esta implementação simples apenas verifica se o objeto aninhado mudou (referência ou null).
                    // Para uma comparação profunda, você chamaria GerarDescricaoDiferencas recursivamente aqui.
                    if (!Equals(valorAntigo, valorNovo))
                    {
                        // Exemplo de como você poderia sinalizar uma mudança em um objeto complexo:
                        // alteracoes.Add($"Campo \"{nomeProp}\" (objeto aninhado) foi alterado.");
                        // Ou, para recursão:
                        // var subAlteracoes = GerarDescricaoDiferencas(valorAntigo as object, valorNovo as object, colecoesNomes);
                        // if (!subAlteracoes.Contains("Nenhuma alteração significativa detectada."))
                        // {
                        //     alteracoes.Add($"Campo \"{nomeProp}\" (objeto aninhado): {subAlteracoes}");
                        // }
                    }
                    continue; // Já tratou a propriedade, vai para a próxima.
                }

                // 3. Lidar com propriedades de tipos primitivos e strings
                CompararPropriedadeSimples(nomeProp, valorAntigo, valorNovo, alteracoes);
            }

            return alteracoes;
        }

        /// <summary>
        /// Compara duas coleções de números (long/int) e adiciona as diferenças à lista de alterações.
        /// </summary>
        private void CompararColecaoDeNumeros(
            string nomeProp,
            IEnumerable? antigoEnumerable,
            IEnumerable? novoEnumerable,
            Dictionary<string, Dictionary<long, string>>? colecoesNomes,
            List<string> alteracoes)
        {
            // Transforma os enumeráveis em listas de long para facilitar a comparação.
            var listaAntiga = antigoEnumerable?.Cast<long>().ToList() ?? new List<long>();
            var listaNova = novoEnumerable?.Cast<long>().ToList() ?? new List<long>();

            // Calcula os itens removidos e adicionados usando LINQ.
            // O .Except() é útil aqui pois a ordem não importa e não queremos duplicatas.
            var removidos = listaAntiga.Except(listaNova).ToList();
            var adicionados = listaNova.Except(listaAntiga).ToList();

            if (removidos.Any() || adicionados.Any())
            {
                var nomesDisponiveis = colecoesNomes != null && colecoesNomes.ContainsKey(nomeProp)
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

        /// <summary>
        /// Compara duas propriedades de tipos simples (primitivos, strings) e adiciona as diferenças à lista de alterações.
        /// </summary>
        private void CompararPropriedadeSimples(string nomeProp, object? valorAntigo, object? valorNovo, List<string> alteracoes)
        {
            // Trata nulos de forma consistente antes de converter para string.
            var strAntigo = valorAntigo?.ToString()?.Trim() ?? "";
            var strNovo = valorNovo?.ToString()?.Trim() ?? "";

            // Compara os valores como strings.
            if (strAntigo != strNovo)
            {
                alteracoes.Add($"Campo \"{nomeProp}\" alterado de \"{strAntigo}\" para \"{strNovo}\" em {DateTime.Now}");
            }
        }

    }
}