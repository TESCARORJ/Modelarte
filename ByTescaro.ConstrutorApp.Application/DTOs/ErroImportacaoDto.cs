namespace ByTescaro.ConstrutorApp.Application.DTOs;

public class ErroImportacaoDto
{
    public string? Mensagem { get; set; }
    public string? Referencia { get; set; }

    public ErroImportacaoDto(string mensagem, string referencia)
    {
        Mensagem = mensagem;
        Referencia = referencia;
    }
}
