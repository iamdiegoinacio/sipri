using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;

namespace SIPRI.Infrastructure.Tests.Persistence.Configurations;

public class ProdutoInvestimentoConfigurationTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void Configuration_ShouldSetTableName()
    {
        // Arrange
        using var context = CreateDbContext();
        var entityType = context.Model.FindEntityType(typeof(ProdutoInvestimento));

        // Act
        var tableName = entityType!.GetTableName();

        // Assert
        tableName.Should().Be("ProdutosInvestimento");
    }

    [Fact]
    public void Configuration_ShouldSetPrimaryKey()
    {
        // Arrange
        using var context = CreateDbContext();
        var entityType = context.Model.FindEntityType(typeof(ProdutoInvestimento));

        // Act
        var primaryKey = entityType!.FindPrimaryKey();

        // Assert
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle();
        primaryKey.Properties.First().Name.Should().Be("Id");
    }

    [Fact]
    public async Task Configuration_ShouldEnforceNomeMaxLength()
    {
        // Arrange
        using var context = CreateDbContext();
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = new string('A', 101), // Excede o limite de 100
            Tipo = "CDB",
            Risco = "Baixo",
            RentabilidadeBase = 0.12m
        };

        context.Produtos.Add(produto);

        // Act & Assert
        // No InMemory database, constraints não são totalmente aplicadas
        // mas a configuração está definida
        var property = context.Model.FindEntityType(typeof(ProdutoInvestimento))!
            .FindProperty("Nome");
        
        property!.GetMaxLength().Should().Be(100);
    }

    [Fact]
    public void Configuration_ShouldSetTipoAsRequired()
    {
        // Arrange
        using var context = CreateDbContext();
        var property = context.Model.FindEntityType(typeof(ProdutoInvestimento))!
            .FindProperty("Tipo");

        // Act
        var isRequired = !property!.IsNullable;

        // Assert
        isRequired.Should().BeTrue();
    }

    [Fact]
    public void Configuration_ShouldSetRentabilidadePrecision()
    {
        // Arrange
        using var context = CreateDbContext();
        var property = context.Model.FindEntityType(typeof(ProdutoInvestimento))!
            .FindProperty("RentabilidadeBase");

        // Act
        var precision = property!.GetPrecision();
        var scale = property.GetScale();

        // Assert
        precision.Should().Be(10);
        scale.Should().Be(4);
    }

    [Theory]
    [InlineData("Nome", 100)]
    [InlineData("Tipo", 50)]
    [InlineData("Risco", 20)]
    public void Configuration_ShouldSetCorrectMaxLengths(string propertyName, int expectedMaxLength)
    {
        // Arrange
        using var context = CreateDbContext();
        var property = context.Model.FindEntityType(typeof(ProdutoInvestimento))!
            .FindProperty(propertyName);

        // Act
        var maxLength = property!.GetMaxLength();

        // Assert
        maxLength.Should().Be(expectedMaxLength);
    }

    [Fact]
    public async Task Configuration_ShouldAllowValidProduct()
    {
        // Arrange
        using var context = CreateDbContext();
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = "CDB Caixa",
            Tipo = "CDB",
            Risco = "Baixo",
            RentabilidadeBase = 0.12m,
            NivelRisco = 1
        };

        // Act
        await context.Produtos.AddAsync(produto);
        var result = await context.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
    }
}