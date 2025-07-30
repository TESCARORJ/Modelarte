namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class SelectableUserDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string TelefoneWhatsApp { get; set; }
        public bool IsSelected { get; set; } = false;

        public string DisplayName => $"{Nome} - {TelefoneWhatsApp}";

    }
}
