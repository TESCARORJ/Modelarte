using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraItemEtapa : EntidadeBase
    {
        public long ObraEtapaId { get; set; }
        public ObraEtapa ObraEtapa { get; set; } 
        public string? Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }

        public bool Concluido { get; set; }
        public bool IsDataPrazo { get; set; }
        public int? DiasPrazo { get; set; }
        public bool PrazoAtivo { get; set; }
        public DateTime? DataConclusao { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }

}
