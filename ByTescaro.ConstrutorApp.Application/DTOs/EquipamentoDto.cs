using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs;

public class EquipamentoDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Patrimonio { get; set; } = string.Empty;
    public StatusEquipamento Status { get; set; }
    public decimal? CustoLocacaoDiaria { get; set; }
    public string UsuarioCadastro { get; set; } = string.Empty;
    public DateTime DataHoraCadastro { get; set; }
    public string ObraNome { get; set; } = string.Empty;
    public string ProjetoNome { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;


}
