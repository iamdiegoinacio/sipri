using FluentAssertions;
using SIPRI.Domain.Contexts;
using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Tests.Contexts;

public class CalculoInvestimentoContextoTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var valor = 1000m;
        var prazo = 12;
        var produto = new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "Teste" };

        // Act
        var contexto = new CalculoInvestimentoContexto(valor, prazo, produto);

        // Assert
        contexto.ValorInvestido.Should().Be(valor);
        contexto.PrazoMeses.Should().Be(prazo);
        contexto.Produto.Should().Be(produto);
    }
}
