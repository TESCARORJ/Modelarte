using ByTescaro.ConstrutorApp.UI.Services;
using Radzen;

namespace ByTescaro.ConstrutorApp.UI.Utils;

public static class EnderecoAutoPreenchimentoHelper
{
    private static readonly Dictionary<string, string> EstadosPorExtenso = new()
    {
        { "AC", "Acre" }, { "AL", "Alagoas" }, { "AP", "Amapá" }, { "AM", "Amazonas" },
        { "BA", "Bahia" }, { "CE", "Ceará" }, { "DF", "Distrito Federal" }, { "ES", "Espírito Santo" },
        { "GO", "Goiás" }, { "MA", "Maranhão" }, { "MT", "Mato Grosso" }, { "MS", "Mato Grosso do Sul" },
        { "MG", "Minas Gerais" }, { "PA", "Pará" }, { "PB", "Paraíba" }, { "PR", "Paraná" },
        { "PE", "Pernambuco" }, { "PI", "Piauí" }, { "RJ", "Rio de Janeiro" }, { "RN", "Rio Grande do Norte" },
        { "RS", "Rio Grande do Sul" }, { "RO", "Rondônia" }, { "RR", "Roraima" }, { "SC", "Santa Catarina" },
        { "SP", "São Paulo" }, { "SE", "Sergipe" }, { "TO", "Tocantins" }
    };

    public static async Task PreencherEnderecoAsync(
        string cep,
        CepService cepService,
        NotificationService notificationService,
        Action<string>? setLogradouro = null,
        Action<string>? setBairro = null,
        Action<string>? setCidade = null,
        Action<string>? setEstado = null,
        Action<string>? setUF = null)
    {
        if (string.IsNullOrWhiteSpace(cep)) return;

        try
        {
            var endereco = await cepService.BuscarAsync(cep);
            if (endereco is null || endereco.Erro == true)
            {
                notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Aviso",
                    Detail = "CEP não encontrado.",
                    Duration = 4000
                });
                return;
            }

            setLogradouro?.Invoke(endereco.Logradouro);
            setBairro?.Invoke(endereco.Bairro);
            setCidade?.Invoke(endereco.Cidade);
            setUF?.Invoke(endereco.UF);

            if (EstadosPorExtenso.TryGetValue(endereco.UF, out var estado))
                setEstado?.Invoke(estado);
        }
        catch
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Erro",
                Detail = "Erro ao buscar o endereço via CEP.",
                Duration = 5000
            });
        }
    }
}
