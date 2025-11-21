using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;
using SIPRI.Infrastructure.Persistence.Repositories;

namespace SIPRI.Infrastructure.Tests.Persistence.Repositories;

public class ProdutoInvestimentoRepositoryTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        using var context = CreateDbContext();
        var id = Guid.NewGuid();
        var produto = new ProdutoInvestimento { Id = id, Nome = "CDB" };
        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();

        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        using var context = CreateDbContext();
        await context.Produtos.AddRangeAsync(
            new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "P1" },
            new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "P2" }
        );
        await context.SaveChangesAsync();

        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }
   
    [Fact]
    public async Task GetByTipoAsync_ShouldReturnProduct_WhenTypeMatches()
    {
        // Arrange
        using var context = CreateDbContext();
        var tipo = "CDB";
        await context.Produtos.AddAsync(new ProdutoInvestimento { Id = Guid.NewGuid(), Tipo = tipo });
        await context.SaveChangesAsync();

        var repository = new ProdutoInvestimentoRepository(context);

        // Act
        var result = await repository.GetByTipoAsync(tipo);

        // Assert
        result.Should().NotBeNull();
        result!.Tipo.Should().Be(tipo);
    }
}
