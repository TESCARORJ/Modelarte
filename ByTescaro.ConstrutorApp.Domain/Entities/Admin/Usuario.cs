using System.Text.Json.Serialization;

namespace ByTescaro.ConstrutorApp.Domain.Entities.Admin
{
    public class Usuario : Pessoa
    {
        public string? SenhaHash { get; set; } = string.Empty;
        public long PerfilUsuarioId { get; set; }

        [JsonIgnore]
        public PerfilUsuario PerfilUsuario { get; set; } = null!;
   
    }
}
