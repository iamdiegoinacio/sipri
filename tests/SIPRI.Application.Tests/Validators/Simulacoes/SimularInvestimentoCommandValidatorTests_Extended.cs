using FluentValidation.TestHelper;
using SIPRI.Application.Commands.Simulacoes;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Application.Validators.Simulacoes;

namespace SIPRI.Application.Tests.Validators.Simulacoes;

public class SimularInvestimentoCommandValidatorTests_Extended
{
    private readonly SimularInvestimentoCommandValidator _validator;

    public SimularInvestimentoCommandValidatorTests_Extended()
    {
        _validator = new SimularInvestimentoCommandValidator();
    }

    [Fact]
    public void Validate_WhenTipoProdutoIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = null!,
            Valor = 10000m,
            PrazoMeses = 12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.TipoProduto)
            .WithErrorMessage("O tipo de produto é obrigatório.");
    }

    [Fact]
    public void Validate_WhenTipoProdutoIsWhiteSpace_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "   ",
            Valor = 10000m,
            PrazoMeses = 12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.TipoProduto)
            .WithErrorMessage("O tipo de produto é obrigatório.");
    }

    [Fact]
    public void Validate_WhenMultipleFieldsAreInvalid_ShouldHaveMultipleErrors()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.Empty,
            TipoProduto = "",
            Valor = -100m,
            PrazoMeses = 0
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.ClienteId);
        result.ShouldHaveValidationErrorFor(x => x.RequestData.TipoProduto);
        result.ShouldHaveValidationErrorFor(x => x.RequestData.Valor);
        result.ShouldHaveValidationErrorFor(x => x.RequestData.PrazoMeses);
    }
}