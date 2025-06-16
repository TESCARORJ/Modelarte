using ByTescaro.ConstrutorApp.Application.DTOs;
using FluentValidation;

namespace ByTescaro.ConstrutorApp.Web.Components.Pages.Funcoes;

public class FuncaoDtoValidator : AbstractValidator<FuncaoDto>
{
    public FuncaoDtoValidator()
    {
        RuleFor(f => f.Nome)
            .NotEmpty().WithMessage("O nome da função é obrigatório")
            .MaximumLength(100).WithMessage("O nome da função deve ter no máximo 100 caracteres");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<FuncaoDto>.CreateWithOptions(
                (FuncaoDto)model, x => x.IncludeProperties(propertyName)));

            return result.IsValid
                ? Array.Empty<string>()
                : result.Errors.Select(e => e.ErrorMessage);
        };
}
