using ByTescaro.ConstrutorApp.Application.DTOs;
using FluentValidation;

namespace ByTescaro.ConstrutorApp.Web.Components.Pages.Equipamentos
{
    public class EquipamentoDtoValidator : AbstractValidator<EquipamentoDto>
    {
        public EquipamentoDtoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do equipamento é obrigatório");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("A descrição é obrigatória");

            RuleFor(x => x.Patrimonio)
                .NotEmpty().WithMessage("O patrimônio é obrigatório");

            RuleFor(x => x.CustoLocacaoDiaria)
                .GreaterThanOrEqualTo(0).WithMessage("O custo de locação deve ser maior ou igual a zero");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status inválido");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
            async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<EquipamentoDto>.CreateWithOptions(
                    (EquipamentoDto)model, x => x.IncludeProperties(propertyName)));

                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
    }
}
