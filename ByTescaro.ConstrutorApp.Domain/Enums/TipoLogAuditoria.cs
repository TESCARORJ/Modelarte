using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum TipoLogAuditoria
    {
        [Display(Name = "criado")]
        Criacao = 1,

        [Display(Name = "atualizado")]
        Atualizacao = 2,

        [Display(Name = "excluído")]
        Exclusao = 3,

        [Display(Name = "consulta")]
        Consulta = 4,

        [Display(Name = "login")]
        Login = 5,

        [Display(Name = "logout")]
        Logout = 6,

        [Display(Name = "erro")]
        Erro = 7,

        [Display(Name = "acesso negado")]
        AcessoNegado = 8,

        [Display(Name = "alteração de senha")]
        AlteracaoSenha = 9,

        [Display(Name = "reset de senha")]
        ResetSenha = 10,

        [Display(Name = "ativação de conta")]
        AtivacaoConta = 11,

        [Display(Name = "desativação de conta")]
        DesativacaoConta = 12,

        [Display(Name = "envio de e-mail")]
        EnvioEmail = 13,

        [Display(Name = "recebimento de e-mail")]
        RecebimentoEmail = 14
    }
}
