using ByTescaro.ConstrutorApp.Application.DTOs;
using FluentValidation;

namespace ByTescaro.ConstrutorApp.Web.Validators
{
    public class UsuarioDtoValidator : AbstractValidator<UsuarioDto>
    {
        public UsuarioDtoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório");

            RuleFor(x => x.Sobrenome)
                .NotEmpty().WithMessage("O sobrenome é obrigatório");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório")
                .EmailAddress().WithMessage("E-mail inválido");

            RuleFor(x => x.Telefone)
                .NotEmpty().WithMessage("O telefone é obrigatório");

            // Senha obrigatória apenas para novo usuário
            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("A senha é obrigatória")
                .MinimumLength(6).WithMessage("A senha deve ter ao menos 6 caracteres")
                .When(x => x.Id == 0 || !string.IsNullOrWhiteSpace(x.Senha));

            RuleFor(x => x.ConfirmarSenha)
                .Equal(x => x.Senha).WithMessage("As senhas não coincidem")
                .When(x => x.Id == 0 || !string.IsNullOrWhiteSpace(x.Senha));

            RuleFor(x => x.PerfilUsuarioId)
                .NotNull().WithMessage("Selecione um perfil");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
            async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<UsuarioDto>.CreateWithOptions(
                    (UsuarioDto)model, x => x.IncludeProperties(propertyName)));

                return result.IsValid
                    ? Array.Empty<string>()
                    : result.Errors.Select(e => e.ErrorMessage);
            };
    }
}
