using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraFuncionario : EntidadeBase
    {
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string UsuarioCadastro { get; set; } = string.Empty;
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;

        public long FuncionarioId { get; set; }
        public string FuncionarioNome { get; set; } = string.Empty;
        public Funcionario Funcionario { get; set; } = default!;

        public string FuncaoNoObra { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }


}
