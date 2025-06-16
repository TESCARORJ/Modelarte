using ByTescaro.ConstrutorApp.Application.DTOs;
using FluentValidation;

namespace ByTescaro.ConstrutorApp.Web.Components.Pages.EtapasObra;

public class ObraItemEtapaPadraoDtoValidator : AbstractValidator<ObraItemEtapaPadraoDto>
{
    public ObraItemEtapaPadraoDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do item é obrigatório")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Ordem)
            .GreaterThan(0).WithMessage("A ordem deve ser maior que zero");

        RuleFor(x => x.ObraEtapaId)
            .GreaterThan(0).WithMessage("Selecione uma etapa da obra");

        When(x => x.IsDataPrazo, () =>
                {
                    RuleFor(x => x.DiasPrazo)
                        .GreaterThan(0).WithMessage("Dias de Prazo deve ser maior que zero quando o controle de prazo está ativo.");
                });
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ObraItemEtapaPadraoDto>.CreateWithOptions((ObraItemEtapaPadraoDto)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
