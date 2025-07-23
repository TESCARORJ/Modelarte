using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{


    public class ProjetoDto
    {
        public long Id { get; set; }
        public string? Nome { get; set; } = string.Empty;
        public DateOnly? DataInicio { get; set; }
        public DateOnly? DataFim { get; set; }
        public StatusProjeto? Status { get; set; }
        public DateOnly? DataInicioExecucao { get; set; }

        public decimal CustoEstimado { get; set; }
        public decimal CustoReal { get; set; }
        public long? ClienteId { get; set; }
        public string? Observacao { get; set; } = string.Empty;
        public string? DescricaoConclusao { get; set; } = string.Empty;
        public string? DescricaoCancelamento { get; set; } = string.Empty;

        // Endereço
        public string? Logradouro { get; set; } = string.Empty;
        public string? Numero { get; set; } = string.Empty;
        public string? Bairro { get; set; } = string.Empty;
        public string? Cidade { get; set; } = string.Empty;
        public string? Estado { get; set; } = string.Empty;
        public string? UF { get; set; } = string.Empty;
        public string? CEP { get; set; } = string.Empty;
        public string? Complemento { get; set; } = string.Empty;
        public string? TelefonePrincipal { get; set; } = string.Empty;

        public long? UsuarioCadastroId { get; set; }
        public string? UsuarioCadastroNome { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }

        public List<ObraDto> Obras { get; set; } = new();

        public int ProgressoProjeto => Obras.Count == 0
            ? 0
            : (int)Math.Round((double)Obras.Sum(o => o.Progresso) / Obras.Count);

    }

}