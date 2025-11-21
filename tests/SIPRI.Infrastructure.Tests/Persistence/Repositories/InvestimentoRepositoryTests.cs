using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;
using SIPRI.Infrastructure.Persistence.Repositories;

namespace SIPRI.Infrastructure.Tests.Persistence.Repositories;

public class InvestimentoRepositoryTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByClienteIdAsync_ShouldReturnInvestments_OrderedByDateDescending()
    {
        // Arrange
        using var context = CreateDbContext();
        var clienteId = Guid.NewGuid();
        
        var inv1 = new Investimento { Id = Guid.NewGuid(), ClienteId = clienteId, Data = DateTime.UtcNow.AddDays(-2), Tipo = "Antigo" };
        var inv2 = new Investimento { Id = Guid.NewGuid(), ClienteId = clienteId, Data = DateTime.UtcNow, Tipo = "Novo" };
        var invOutroCliente = new Investimento { Id = Guid.NewGuid(), ClienteId = Guid.NewGuid(), Data = DateTime.UtcNow, Tipo = "Outro" };

        await context.Investimentos.AddRangeAsync(inv1, inv2, invOutroCliente);
        await context.SaveChangesAsync();

        var repository = new InvestimentoRepository(context);

        // Act
        var result = await repository.GetByClienteIdAsync(clienteId);

        // Assert
        result.Should().HaveCount(2);
        result.First().Tipo.Should().Be("Novo"); // Most recent first
        result.Last().Tipo.Should().Be("Antigo");
    }

    [Fact]
    public async Task GetByClienteIdAsync_ShouldReturnEmpty_WhenNoInvestmentsFound()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new InvestimentoRepository(context);

        // Act
        var result = await repository.GetByClienteIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }
}
