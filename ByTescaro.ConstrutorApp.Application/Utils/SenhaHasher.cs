using System.Security.Cryptography;
using System.Text;

namespace ByTescaro.ConstrutorApp.Application.Utils
{
    public static class SenhaHasher
    {
        public static string GerarHash(string senha)
        {
            if (string.IsNullOrEmpty(senha))
                throw new ArgumentException("A senha não pode ser nula ou vazia.", nameof(senha));

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
