
using ByTescaro.ConstrutorApp.Application.DTOs;
using FluentValidation;

namespace ByTescaro.ConstrutorApp.Web.Components.Pages.EtapasObra;

public class ObraEtapaPadraoDtoValidator : AbstractValidator<ObraEtapaDto>
{
    public ObraEtapaPadraoDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da etapa é obrigatório")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres");

        RuleFor(x => x.Ordem)
            .GreaterThan(0).WithMessage("A ordem deve ser maior que zero");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ObraEtapaDto>.CreateWithOptions(
                (ObraEtapaDto)model, x => x.IncludeProperties(propertyName)));

            return result.IsValid
                ? Array.Empty<string>()
                : result.Errors.Select(e => e.ErrorMessage);
        };
}
