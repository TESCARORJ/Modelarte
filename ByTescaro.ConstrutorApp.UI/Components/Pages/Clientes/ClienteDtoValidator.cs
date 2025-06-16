using ByTescaro.ConstrutorApp.Application.DTOs;
using FluentValidation;

namespace ByTescaro.ConstrutorApp.Web.Components.Pages.Clientes
{
    public class ClienteDtoValidator : AbstractValidator<ClienteDto>
    {
        public ClienteDtoValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome é obrigatório");
            RuleFor(x => x.TipoPessoa).IsInEnum().WithMessage("Tipo de pessoa inválido");
            RuleFor(x => x.CpfCnpj).NotEmpty().WithMessage("O CPF/CNPJ é obrigatório");

            RuleFor(x => x.Logradouro).NotEmpty();
            RuleFor(x => x.Numero).NotEmpty();
            RuleFor(x => x.Bairro).NotEmpty();
            RuleFor(x => x.Cidade).NotEmpty();
            RuleFor(x => x.Estado).NotEmpty();
            RuleFor(x => x.UF).NotEmpty().Length(2).WithMessage("UF deve ter 2 letras");
            RuleFor(x => x.CEP).NotEmpty().WithMessage("O CEP é obrigatório");

            RuleFor(x => x.TelefonePrincipal).NotEmpty().WithMessage("O telefone principal é obrigatório");
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("E-mail inválido");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
            async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<ClienteDto>.CreateWithOptions(
                    (ClienteDto)model, x => x.IncludeProperties(propertyName)));

                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
    }
}
