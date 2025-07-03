namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Funcionario : Pessoa
    {
        public decimal Salario { get; set; }
        public DateTime DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }

        public long FuncaoId { get; set; }
        public Funcao Funcao { get; set; } = default!;

        public ICollection<ObraFuncionario> ProjetoFuncionarios { get; set; } = new List<ObraFuncionario>();

    }
}
