using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum TipoLogAuditoria
    {
        Criacao = 1,
        Atualizacao = 2,
        Exclusao = 3,
        Consulta = 4,
        Login = 5,
        Logout = 6,
        Erro = 7,
        AcessoNegado = 8,
        AlteracaoSenha = 9,
        ResetSenha = 10,
        AtivacaoConta = 11,
        DesativacaoConta = 12,
        EnvioEmail = 13,
        RecebimentoEmail = 14
    }
}
