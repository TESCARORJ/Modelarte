//using ByTescaro.ConstrutorApp.Application.DTOs;
//using FluentValidation;

//namespace ByTescaro.ConstrutorApp.Web.Components.Pages.Localizacao
//{
//    public class LocalizacaoEstoqueDtoValidator : AbstractValidator<LocalizacaoEstoqueDto>
//    {
//        public LocalizacaoEstoqueDtoValidator()
//        {
//            RuleFor(x => x.Nome)
//                .NotEmpty().WithMessage("O nome da localização é obrigatório")
//                .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres");

//            RuleFor(x => x.Descricao)
//                .MaximumLength(250).WithMessage("A descrição deve ter no máximo 250 caracteres");
//        }

//        public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
//            async (model, propertyName) =>
//            {
//                var result = await ValidateAsync(ValidationContext<LocalizacaoEstoqueDto>.CreateWithOptions(
//                    (LocalizacaoEstoqueDto)model, x => x.IncludeProperties(propertyName)));

//                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
//            };
//    }
//}
