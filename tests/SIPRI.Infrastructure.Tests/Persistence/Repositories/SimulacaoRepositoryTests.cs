using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;
using SIPRI.Infrastructure.Persistence.Repositories;

namespace SIPRI.Infrastructure.Tests.Persistence.Repositories;

public class SimulacaoRepositoryTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddSimulationToContext()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new SimulacaoRepository(context);
        var simulacao = new Simulacao { Id = Guid.NewGuid(), ProdutoNome = "Teste" };

        // Act
        await repository.AddAsync(simulacao);
        await context.SaveChangesAsync();

        // Assert
        context.Simulacoes.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByClienteIdAsync_ShouldReturnSimulations_OrderedByDateDescending()
    {
        // Arrange
        using var context = CreateDbContext();
        var clienteId = Guid.NewGuid();
        
        var s1 = new Simulacao { Id = Guid.NewGuid(), ClienteId = clienteId, DataSimulacao = DateTime.UtcNow.AddDays(-1), ProdutoNome = "Antiga" };
        var s2 = new Simulacao { Id = Guid.NewGuid(), ClienteId = clienteId, DataSimulacao = DateTime.UtcNow, ProdutoNome = "Nova" };
        
        await context.Simulacoes.AddRangeAsync(s1, s2);
        await context.SaveChangesAsync();

        var repository = new SimulacaoRepository(context);

        // Act
        var result = await repository.GetByClienteIdAsync(clienteId);

        // Assert
        result.Should().HaveCount(2);
        result.First().ProdutoNome.Should().Be("Nova");
    }

    [Fact]
    public async Task GetAgregadoPorDiaAsync_ShouldAggregateCorrectly()
    {
        // Arrange
        using var context = CreateDbContext();
        var hoje = DateTime.UtcNow.Date;
        
        // 2 simulações de "CDB" hoje
        await context.Simulacoes.AddAsync(new Simulacao { Id = Guid.NewGuid(), ProdutoNome = "CDB", DataSimulacao = hoje, ValorFinal = 100 });
        await context.Simulacoes.AddAsync(new Simulacao { Id = Guid.NewGuid(), ProdutoNome = "CDB", DataSimulacao = hoje, ValorFinal = 200 });
        
        // 1 simulação de "Fundo" hoje
        await context.Simulacoes.AddAsync(new Simulacao { Id = Guid.NewGuid(), ProdutoNome = "Fundo", DataSimulacao = hoje, ValorFinal = 300 });

        await context.SaveChangesAsync();

        var repository = new SimulacaoRepository(context);

        // Act
        var result = await repository.GetAgregadoPorDiaAsync();

        // Assert
        result.Should().HaveCount(2);
        
        var cdbStats = result.First(x => x.Produto == "CDB");
        cdbStats.QuantidadeSimulacoes.Should().Be(2);
        cdbStats.MediaValorFinal.Should().Be(150); // (100+200)/2

        var fundoStats = result.First(x => x.Produto == "Fundo");
        fundoStats.QuantidadeSimulacoes.Should().Be(1);
        fundoStats.MediaValorFinal.Should().Be(300);
    }
}
