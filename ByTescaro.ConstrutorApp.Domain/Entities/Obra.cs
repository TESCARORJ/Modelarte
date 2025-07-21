using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
namespace ByTescaro.ConstrutorApp.Domain.Entities
{

    public class Obra : EntidadeBase
    {
        public long ProjetoId { get; set; }
        public Projeto Projeto { get; set; } = null!;
        public string? Nome { get; set; } = string.Empty;
        public StatusObra Status { get; set; }
        public DateTime? DataInicioExecucao { get; set; }    

        public ResponsavelMaterialEnum ResponsavelMaterial { get; set; }
        public long? ResponsavelObraId { get; set; }
        public Funcionario? ResponsavelObra { get; set; } = null!;

       public Usuario? UsuarioCadastro { get; set; }

        public ICollection<ObraEtapa> Etapas { get; set; } = new List<ObraEtapa>();
        public ICollection<ObraFuncionario> Funcionarios { get; set; } = new List<ObraFuncionario>();
        public ICollection<ObraFornecedor> Fornecedores { get; set; } = new List<ObraFornecedor>();
        public ICollection<ObraInsumo> Insumos { get; set; } = new List<ObraInsumo>();
        public ICollection<ObraInsumoLista> ListasInsumo { get; set; } = new List<ObraInsumoLista>();

        public ICollection<ObraEquipamento> Equipamentos { get; set; } = new List<ObraEquipamento>();
        public ICollection<ObraRetrabalho> Retrabalhos { get; set; } = new List<ObraRetrabalho>();
        public ICollection<ObraPendencia> Pendencias { get; set; } = new List<ObraPendencia>();
        public ICollection<ObraDocumento> Documentos { get; set; } = new List<ObraDocumento>();
        public ICollection<ObraImagem> Imagens { get; set; } = new List<ObraImagem>();
        public ICollection<ObraServico> Servicos { get; set; } = new List<ObraServico>();
        public ICollection<ObraServicoLista> ListasServico { get; set; } = new List<ObraServicoLista>();

    }

}