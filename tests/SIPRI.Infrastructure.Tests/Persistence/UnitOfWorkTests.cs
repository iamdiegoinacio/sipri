using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;
using SIPRI.Infrastructure.Persistence.Repositories;

namespace SIPRI.Infrastructure.Tests.Persistence;

public class UnitOfWorkTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistAllChanges()
    {
        // Arrange
        using var context = CreateDbContext();
        var unitOfWork = new UnitOfWork(context);
        
        var produto = new ProdutoInvestimento
        {
            Id = Guid.NewGuid(),
            Nome = "Teste",
            Tipo = "CDB"
        };

        await context.Produtos.AddAsync(produto);

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(1); // 1 entidade salva
        var saved = await context.Produtos.FindAsync(produto.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnZero_WhenNoChanges()
    {
        // Arrange
        using var context = CreateDbContext();
        var unitOfWork = new UnitOfWork(context);

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldHandleMultipleEntities()
    {
        // Arrange
        using var context = CreateDbContext();
        var unitOfWork = new UnitOfWork(context);

        var produto1 = new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "P1" };
        var produto2 = new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "P2" };
        var simulacao = new Simulacao { Id = Guid.NewGuid(), ProdutoNome = "S1", ClienteId = Guid.NewGuid() };

        await context.Produtos.AddRangeAsync(produto1, produto2);
        await context.Simulacoes.AddAsync(simulacao);

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(3); // 3 entidades salvas
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSupportTransactionalBehavior()
    {
        // Arrange
        using var context = CreateDbContext();
        var unitOfWork = new UnitOfWork(context);

        var produto = new ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "Test" };
        await context.Produtos.AddAsync(produto);

        // Act
        await unitOfWork.SaveChangesAsync();
        
        produto.Nome = "Updated";
        await unitOfWork.SaveChangesAsync();

        // Assert
        var saved = await context.Produtos.FindAsync(produto.Id);
        saved!.Nome.Should().Be("Updated");
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPropagateExceptions()
    {
        // Arrange
        using var context = CreateDbContext();
        var unitOfWork = new UnitOfWork(context);

        // Cria uma entidade com dados inválidos (simulando violação de constraint)
        var produto = new ProdutoInvestimento
        {
            Id = Guid.Empty, // ID vazio pode causar problemas
            Nome = null! // Nome é obrigatório
        };

        await context.Produtos.AddAsync(produto);

        // Act
        Func<Task> act = async () => await unitOfWork.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }
}