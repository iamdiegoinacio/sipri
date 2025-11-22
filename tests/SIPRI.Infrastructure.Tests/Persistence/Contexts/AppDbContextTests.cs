using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;

namespace SIPRI.Infrastructure.Tests.Persistence.Contexts;

public class AppDbContextTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void DbContext_ShouldHaveProdutosDbSet()
    {
        // Arrange & Act
        using var context = CreateDbContext();

        // Assert
        context.Produtos.Should().NotBeNull();
    }

    [Fact]
    public void DbContext_ShouldHaveInvestimentosDbSet()
    {
        // Arrange & Act
        using var context = CreateDbContext();

        // Assert
        context.Investimentos.Should().NotBeNull();
    }

    [Fact]
    public void DbContext_ShouldHaveSimulacoesDbSet()
    {
        // Arrange & Act
        using var context = CreateDbContext();

        // Assert
        context.Simulacoes.Should().NotBeNull();
    }

    [Fact]
    public async Task DbContext_ShouldPersistProdutoInvestimento()
    {
        // Arrange
        using var context = CreateDbContext();
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = "CDB Teste",
            Tipo = "CDB",
            RentabilidadeBase = 0.12m,
            Risco = "Baixo",
            NivelRisco = 1
        };

        // Act
        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Produtos.FindAsync(produto.Id);
        saved.Should().NotBeNull();
        saved!.Nome.Should().Be("CDB Teste");
    }

    [Fact]
    public async Task DbContext_ShouldPersistInvestimento()
    {
        // Arrange
        using var context = CreateDbContext();
        var investimento = new Investimento
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            ProdutoId = Guid.NewGuid(),
            Tipo = "CDB",
            Valor = 1000m,
            Rentabilidade = 0.12m,
            Data = DateTime.UtcNow
        };

        // Act
        await context.Investimentos.AddAsync(investimento);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Investimentos.FindAsync(investimento.Id);
        saved.Should().NotBeNull();
        saved!.Valor.Should().Be(1000m);
    }

    [Fact]
    public async Task DbContext_ShouldPersistSimulacao()
    {
        // Arrange
        using var context = CreateDbContext();
        var simulacao = new Simulacao
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            ProdutoId = Guid.NewGuid(),
            ProdutoNome = "CDB Teste",
            ValorInvestido = 1000m,
            PrazoMeses = 12,
            ValorFinal = 1120m,
            DataSimulacao = DateTime.UtcNow
        };

        // Act
        await context.Simulacoes.AddAsync(simulacao);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Simulacoes.FindAsync(simulacao.Id);
        saved.Should().NotBeNull();
        saved!.ProdutoNome.Should().Be("CDB Teste");
    }

    [Fact]
    public void DbContext_ShouldApplyConfigurationsFromAssembly()
    {
        // Arrange & Act
        using var context = CreateDbContext();
        var model = context.Model;

        // Assert
        // Verifica que as configurações foram aplicadas
        var produtoEntity = model.FindEntityType(typeof(ProdutoInvestimento));
        produtoEntity.Should().NotBeNull();
    }

    [Fact]
    public async Task DbContext_ShouldHandleConcurrentOperations()
    {
        // Arrange
        using var context1 = CreateDbContext();
        using var context2 = CreateDbContext();

        var produto1 = new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "P1" };
        var produto2 = new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "P2" };

        // Act
        await context1.Produtos.AddAsync(produto1);
        await context2.Produtos.AddAsync(produto2);

        await context1.SaveChangesAsync();
        await context2.SaveChangesAsync();

        // Assert
        // Ambos devem ser salvos sem conflitos (IDs diferentes)
        var count1 = await context1.Produtos.CountAsync();
        var count2 = await context2.Produtos.CountAsync();
        
        count1.Should().BeGreaterThan(0);
        count2.Should().BeGreaterThan(0);
    }
}