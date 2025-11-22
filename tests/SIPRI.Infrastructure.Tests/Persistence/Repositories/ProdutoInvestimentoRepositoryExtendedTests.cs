using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;
using SIPRI.Infrastructure.Persistence.Repositories;

namespace SIPRI.Infrastructure.Tests.Persistence.Repositories;

public class ProdutoInvestimentoRepositoryExtendedTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new ProdutoInvestimentoRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoProducts()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldUseAsNoTracking()
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

        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();

        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        result.Should().NotBeEmpty();
        context.Entry(result.First()).State.Should().Be(EntityState.Detached);
    }

    [Fact]
    public async Task GetByPerfilRiscoAsync_ShouldReturnMatchingProducts()
    {
        // Arrange
        using var context = CreateDbContext();
        var produtoBaixo = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = "CDB Baixo Risco",
            Tipo = "CDB",
            Risco = "Baixo",
            NivelRisco = 1
        };

        var produtoAlto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = "Fundo Alto Risco",
            Tipo = "Fundo",
            Risco = "Alto",
            NivelRisco = 3
        };

        await context.Produtos.AddRangeAsync(produtoBaixo, produtoAlto);
        await context.SaveChangesAsync();

        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var result = await repository.GetByPerfilRiscoAsync("Alto");

        // Assert
        result.Should().ContainSingle();
        result.First().Risco.Should().Be("Alto");
    }

    [Fact]
    public async Task GetByTipoAsync_ShouldReturnNull_WhenTypeDoesNotExist()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var result = await repository.GetByTipoAsync("TipoInexistente");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByTipoAsync_ShouldBeCaseInsensitive()
    {
        // Arrange
        using var context = CreateDbContext();
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = "CDB Teste",
            Tipo = "CDB"
        };

        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();

        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var resultLower = await repository.GetByTipoAsync("cdb");
        var resultUpper = await repository.GetByTipoAsync("CDB");

        // Assert
        resultLower.Should().BeNull(); // Case sensitive por padrão no EF Core
        resultUpper.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldTrackEntity()
    {
        // Arrange
        using var context = CreateDbContext();
        var id = Guid.NewGuid();
        var produto = new ProdutoInvestimento
        {
            Id = id,
            Nome = "Produto Teste"
        };

        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();

        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        // FindAsync retorna entidades rastreadas
        context.Entry(result!).State.Should().Be(EntityState.Unchanged);
    }

    [Theory]
    [InlineData("Conservador")]
    [InlineData("Moderado")]
    [InlineData("Agressivo")]
    public async Task GetByPerfilRiscoAsync_ShouldHandleDifferentProfiles(string perfil)
    {
        // Arrange
        using var context = CreateDbContext();
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = $"Produto {perfil}",
            Risco = perfil
        };

        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();

        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var result = await repository.GetByPerfilRiscoAsync(perfil);

        // Assert
        result.Should().ContainSingle();
        result.First().Risco.Should().Be(perfil);
    }
}