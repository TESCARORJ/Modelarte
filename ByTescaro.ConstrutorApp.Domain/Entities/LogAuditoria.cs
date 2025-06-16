using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class LogAuditoria
    {
        public long Id { get; set; }

        public string Usuario { get; set; } = string.Empty;
        public string Entidade { get; set; } = string.Empty;
        public string Acao { get; set; } = string.Empty; // Criado, Atualizado, Excluído
        public string Descricao { get; set; } = string.Empty; // Ex: "Cliente João Silva foi atualizado"
        public DateTime DataHora { get; set; } = DateTime.Now;

        public string? DadosAnteriores { get; set; } // JSON
        public string? DadosAtuais { get; set; } // JSON

        public string? IdEntidade { get; set; } // ID da entidade

    }
}
