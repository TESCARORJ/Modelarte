//using ByTescaro.ConstrutorApp.Application.DTOs;
//using ByTescaro.ConstrutorApp.Domain.Enums;
//using FluentValidation;

//namespace ByTescaro.ConstrutorApp.Web.Components.Pages.Insumos
//{
//    public class InsumoDtoValidator : AbstractValidator<InsumoDto>
//    {
//        public InsumoDtoValidator()
//        {
//            RuleFor(x => x.Nome)
//                .NotEmpty().WithMessage("O nome do insumo é obrigatório");

//            RuleFor(x => x.UnidadeMedida)
//                .IsInEnum().WithMessage("Unidade de medida inválida")
//                .NotEqual(default(UnidadeMedida)).WithMessage("Selecione uma unidade de medida");


//        }

//        public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
//            async (model, propertyName) =>
//            {
//                var result = await ValidateAsync(ValidationContext<InsumoDto>.CreateWithOptions(
//                    (InsumoDto)model, x => x.IncludeProperties(propertyName)));

//                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
//            };
//    }
//}
