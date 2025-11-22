using FluentValidation.TestHelper;
using SIPRI.Application.Commands.Simulacoes;
using SIPRI.Application.DTOs.Simulacoes;
using SIPRI.Application.Validators.Simulacoes;
using Xunit;

namespace SIPRI.Application.Tests.Validators.Simulacoes;

public class SimularInvestimentoCommandValidatorTests
{
    private readonly SimularInvestimentoCommandValidator _validator;

    public SimularInvestimentoCommandValidatorTests()
    {
        _validator = new SimularInvestimentoCommandValidator();
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 10000m,
            PrazoMeses = 12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenRequestDataIsNull_ShouldHaveValidationError()
    {
        // Arrange
        // Arrange
        // O construtor lança ArgumentNullException se requestData for null, 
        // então este teste verifica o comportamento do construtor ou deve ser removido se o objetivo for validar o validator.
        // Como estamos testando o Validator, e o Command garante não nulo, este teste de validação se torna obsoleto ou deve testar propriedades nulas dentro do DTO.
        // Vou comentar ou remover este teste se ele não fizer mais sentido.
        // Mas para manter a compilação, vou ajustar para tentar passar null se possível, ou remover.
        // O construtor tem: RequestData = requestData ?? throw new ArgumentNullException...
        // Então não é possível criar um command com RequestData null.
        // Vou remover este teste ou alterá-lo para verificar ArgumentNullException na criação do comando.
        
        Assert.Throws<ArgumentNullException>(() => new SimularInvestimentoCommand(null!));
        return; 

    }

    [Fact]
    public void Validate_WhenClienteIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.Empty,
            TipoProduto = "CDB",
            Valor = 10000m,
            PrazoMeses = 12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.ClienteId)
            .WithErrorMessage("O ID do cliente é obrigatório.");
    }

    [Fact]
    public void Validate_WhenTipoProdutoIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = string.Empty,
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
    public void Validate_WhenTipoProdutoExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = new string('A', 51), // 51 caracteres
            Valor = 10000m,
            PrazoMeses = 12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.TipoProduto)
            .WithErrorMessage("O tipo de produto não pode exceder 50 caracteres.");
    }

    [Fact]
    public void Validate_WhenValorIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 0m,
            PrazoMeses = 12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.Valor)
            .WithErrorMessage("O valor do investimento deve ser maior que zero.");
    }

    [Fact]
    public void Validate_WhenValorIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = -1000m,
            PrazoMeses = 12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.Valor)
            .WithErrorMessage("O valor do investimento deve ser maior que zero.");
    }

    [Fact]
    public void Validate_WhenValorExceedsMaximum_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 1_000_000_001m, // Acima do limite
            PrazoMeses = 12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.Valor)
            .WithErrorMessage("O valor do investimento não pode exceder R$ 1.000.000.000,00.");
    }

    [Fact]
    public void Validate_WhenPrazoMesesIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 10000m,
            PrazoMeses = 0
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.PrazoMeses)
            .WithErrorMessage("O prazo deve ser maior que zero meses.");
    }

    [Fact]
    public void Validate_WhenPrazoMesesIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 10000m,
            PrazoMeses = -12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.PrazoMeses)
            .WithErrorMessage("O prazo deve ser maior que zero meses.");
    }

    [Fact]
    public void Validate_WhenPrazoMesesExceedsMaximum_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 10000m,
            PrazoMeses = 361 // Acima de 360 meses
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestData.PrazoMeses)
            .WithErrorMessage("O prazo não pode exceder 360 meses (30 anos).");
    }

    [Fact]
    public void Validate_WhenMultipleFieldsAreInvalid_ShouldReturnMultipleErrors()
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.Empty,
            TipoProduto = string.Empty,
            Valor = 0m,
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

    [Theory]
    [InlineData(0.01)]
    [InlineData(100)]
    [InlineData(10000)]
    [InlineData(1000000)]
    [InlineData(999999999)]
    public void Validate_WhenValorIsWithinValidRange_ShouldNotHaveValidationError(decimal valor)
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = valor,
            PrazoMeses = 12
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RequestData.Valor);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(24)]
    [InlineData(360)]
    public void Validate_WhenPrazoMesesIsWithinValidRange_ShouldNotHaveValidationError(int prazoMeses)
    {
        // Arrange
        var command = new SimularInvestimentoCommand(new SimulacaoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            TipoProduto = "CDB",
            Valor = 10000m,
            PrazoMeses = prazoMeses
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RequestData.PrazoMeses);
    }
}
