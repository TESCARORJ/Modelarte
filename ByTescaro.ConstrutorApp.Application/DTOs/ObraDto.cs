using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraDto
    {
        public long Id { get; set; }
        public long ProjetoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public StatusObra? Status { get; set; }
        public DateTime? DataInicioExecucao { get; set; }


        // Endereço alternativo (opcional)
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? UF { get; set; }
        public string? CEP { get; set; }
        public string? Complemento { get; set; }

        public ResponsavelMaterialEnum? ResponsavelMaterial { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }

        // Relacionamentos
        public List<ObraFuncionarioDto> Funcionarios { get; set; } = new();
        public List<ObraFornecedorDto> Fornecedores { get; set; } = new();
        public List<ObraInsumoDto> Insumos { get; set; } = new();
        public List<ObraInsumoListaDto> ListasInsumo { get; set; } = new();    
        public List<ObraServicoDto> Servicos { get; set; } = new();
        public List<ObraServicoListaDto> ListasServico { get; set; } = new();

        public List<ObraEquipamentoDto> Equipamentos { get; set; } = new();
        public List<ObraEtapaDto> Etapas { get; set; } = new();
        public List<ObraRetrabalhoDto> Retrabalhos { get; set; } = new();
        public List<ObraDocumentoDto> Documentos { get; set; } = new();
        public List<ObraImagemDto> Imagens { get; set; } = new();
        public List<ObraPendenciaDto> Pendencias { get; set; } = new();

        public int Progresso =>
            Etapas.Count == 0
            ? 0
            : (int)Math.Round((double)Etapas.Sum(e => e.Progresso) / Etapas.Count);
    }

}
