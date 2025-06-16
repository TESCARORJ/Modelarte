using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Domain.Enums;
using FluentValidation;

namespace ByTescaro.ConstrutorApp.UI.Components.Pages.Projetos
{
    public class ProjetoDtoValidator : AbstractValidator<ProjetoDto>
    {
        public ProjetoDtoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do projeto é obrigatório");

            RuleFor(x => x.DataInicio)
                .NotNull().WithMessage("A data de início é obrigatória");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status inválido")
                .NotEqual(default(StatusProjeto)).WithMessage("Selecione um status válido");

            RuleFor(x => x.CustoEstimado)
                .GreaterThanOrEqualTo(0).WithMessage("O custo estimado deve ser maior ou igual a zero");

            RuleFor(x => x.CustoReal)
                .GreaterThanOrEqualTo(0).WithMessage("O custo real deve ser maior ou igual a zero");

            RuleFor(x => x.ClienteId)
                .GreaterThan(0).WithMessage("Selecione um cliente");

            RuleFor(x => x.CEP)
                .NotEmpty().WithMessage("O CEP é obrigatório");

            //RuleFor(x => x.Logradouro)
            //    .NotEmpty().WithMessage("O logradouro é obrigatório");

            RuleFor(x => x.Numero)
                .NotEmpty().WithMessage("O número é obrigatório");

            //RuleFor(x => x.Bairro)
            //    .NotEmpty().WithMessage("O bairro é obrigatório");

            //RuleFor(x => x.Cidade)
            //    .NotEmpty().WithMessage("A cidade é obrigatória");

            //RuleFor(x => x.Estado)
            //    .NotEmpty().WithMessage("O estado é obrigatório");

            //RuleFor(x => x.UF)
            //    .NotEmpty().WithMessage("A UF é obrigatória");

            RuleFor(x => x.TelefonePrincipal)
                .NotEmpty().WithMessage("O telefone principal é obrigatório");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
            async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<ProjetoDto>.CreateWithOptions(
                    (ProjetoDto)model, x => x.IncludeProperties(propertyName)));

                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
    }
}
