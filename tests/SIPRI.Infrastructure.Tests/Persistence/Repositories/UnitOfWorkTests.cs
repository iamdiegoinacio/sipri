using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIPRI.Infrastructure.Persistence.Contexts;
using SIPRI.Infrastructure.Persistence.Repositories;

namespace SIPRI.Infrastructure.Tests.Persistence.Repositories;

public class UnitOfWorkTests
{
    [Fact]
    public async Task SaveChangesAsync_ShouldCommitChangesToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        using var context = new AppDbContext(options);
        var unitOfWork = new UnitOfWork(context);

        // Add an entity directly to context to simulate work
        // We need an entity to add. I'll use ProdutoInvestimento as it's simple.
        var produto = new SIPRI.Domain.Entities.ProdutoInvestimento { Id = Guid.NewGuid(), Nome = "Teste" };
        await context.Produtos.AddAsync(produto);

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(1); // 1 record affected
        
        // Verify it's saved
        using var context2 = new AppDbContext(options);
        context2.Produtos.Should().ContainSingle();
    }
}
