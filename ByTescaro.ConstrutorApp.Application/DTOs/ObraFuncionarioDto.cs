namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraFuncionarioDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public long FuncionarioId { get; set; }
        public string? FuncionarioNome { get; set; } = string.Empty;
        public string? FuncaoNoObra { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public long UsuarioCadastroId { get; set; }
        public string? UsuarioCadastroNome { get; set; } = string.Empty;
    }

}
