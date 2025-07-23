using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraFuncionario : EntidadeBase
    {

        public long ObraId { get; set; }
        public Obra Obra { get; set; }

        public long FuncionarioId { get; set; }
        public string? FuncionarioNome { get; set; } = string.Empty;
        public Funcionario Funcionario { get; set; } 

        public string? FuncaoNoObra { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }


}
