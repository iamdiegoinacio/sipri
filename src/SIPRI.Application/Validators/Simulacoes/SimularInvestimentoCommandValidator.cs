using FluentValidation;
using SIPRI.Application.Commands.Simulacoes;

namespace SIPRI.Application.Validators.Simulacoes;

/// <summary>
/// Validador FluentValidation para SimularInvestimentoCommand.
/// Valida os dados de entrada antes do comando ser processado pelo handler.
/// </summary>
public class SimularInvestimentoCommandValidator : AbstractValidator<SimularInvestimentoCommand>
{
    public SimularInvestimentoCommandValidator()
    {
        // Validação do objeto RequestData
        RuleFor(x => x.RequestData)
            .NotNull()
            .WithMessage("Os dados da simulação são obrigatórios.");

        // Validações dos campos do RequestData
        When(x => x.RequestData != null, () =>
        {
            RuleFor(x => x.RequestData.ClienteId)
                .NotEmpty()
                .WithMessage("O ID do cliente é obrigatório.");

            RuleFor(x => x.RequestData.TipoProduto)
                .NotEmpty()
                .WithMessage("O tipo de produto é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O tipo de produto não pode exceder 50 caracteres.");

            RuleFor(x => x.RequestData.Valor)
                .GreaterThan(0)
                .WithMessage("O valor do investimento deve ser maior que zero.")
                .LessThanOrEqualTo(1_000_000_000)
                .WithMessage("O valor do investimento não pode exceder R$ 1.000.000.000,00.");

            RuleFor(x => x.RequestData.PrazoMeses)
                .GreaterThan(0)
                .WithMessage("O prazo deve ser maior que zero meses.")
                .LessThanOrEqualTo(360)
                .WithMessage("O prazo não pode exceder 360 meses (30 anos).");
        });
    }
}
