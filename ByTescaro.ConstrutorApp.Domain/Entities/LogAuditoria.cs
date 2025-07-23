using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class LogAuditoria
    {
        public long Id { get; set; }
        public long UsuarioId { get; set; }
        public string? UsuarioNome { get; set; } = string.Empty;
        public string? Entidade { get; set; } = string.Empty;
        public TipoLogAuditoria TipoLogAuditoria { get; set; } // Criado, Atualizado, Excluído
        public string? Descricao { get; set; } = string.Empty; // Ex: "Cliente João Silva foi atualizado"
        public DateTime DataHora { get; set; } = DateTime.Now;
        public string? DadosAnteriores { get; set; } // JSON
        public string? DadosAtuais { get; set; } // JSON
        public string? IdEntidade { get; set; } // ID da entidade

    }
}
