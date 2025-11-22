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
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByClienteIdAsync_ShouldReturnInvestments_WhenClienteHasInvestments()
    {
        // Arrange
        using var context = CreateDbContext();
        var clienteId = Guid.NewGuid();
        var investimento1 = new Investimento
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Tipo = "CDB",
            Valor = 1000m,
            Data = DateTime.UtcNow.AddDays(-1)
        };
        var investimento2 = new Investimento
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Tipo = "Fundo",
            Valor = 2000m,
            Data = DateTime.UtcNow
        };

        await context.Investimentos.AddRangeAsync(investimento1, investimento2);
        await context.SaveChangesAsync();

        var repository = new InvestimentoRepository(context);

        // Act
        var result = await repository.GetByClienteIdAsync(clienteId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(i => i.Id == investimento1.Id);
        result.Should().Contain(i => i.Id == investimento2.Id);
    }

    [Fact]
    public async Task GetByClienteIdAsync_ShouldReturnEmpty_WhenClienteHasNoInvestments()
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
