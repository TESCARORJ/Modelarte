using ByTescaro.ConstrutorApp.Application.DTOs;
using FluentValidation;

namespace ByTescaro.ConstrutorApp.Web.Components.Pages.PerfilUsuarios
{
    public class PerfilUsuarioDtoValidator : AbstractValidator<PerfilUsuarioDto>
    {
        public PerfilUsuarioDtoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do perfilUsuario é obrigatório");
           
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
            async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<PerfilUsuarioDto>.CreateWithOptions(
                    (PerfilUsuarioDto)model, x => x.IncludeProperties(propertyName)));

                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
    }
}
