using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;
using SIPRI.Infrastructure.Persistence.Repositories;

namespace SIPRI.Infrastructure.Tests.Persistence.Repositories;

public class SimulacaoRepositoryExtendedTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldNotAutoSave()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new SimulacaoRepository(context);
        var simulacao = new Simulacao
        {
            Id = Guid.NewGuid(),
            ProdutoNome = "Teste",
            ClienteId = Guid.NewGuid()
        };

        // Act
        await repository.AddAsync(simulacao);

        // Assert
        // Sem SaveChanges, a entidade não deve estar no banco
        var count = await context.Simulacoes.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoSimulations()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new SimulacaoRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSimulations()
    {
        // Arrange
        using var context = CreateDbContext();
        var simulacoes = new List<Simulacao>
        {
            new Simulacao { Id = Guid.NewGuid(), ProdutoNome = "Sim1", ClienteId = Guid.NewGuid() },
            new Simulacao { Id = Guid.NewGuid(), ProdutoNome = "Sim2", ClienteId = Guid.NewGuid() },
            new Simulacao { Id = Guid.NewGuid(), ProdutoNome = "Sim3", ClienteId = Guid.NewGuid() }
        };

        await context.Simulacoes.AddRangeAsync(simulacoes);
        await context.SaveChangesAsync();

        var repository = new SimulacaoRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetByClienteIdAsync_ShouldReturnEmpty_WhenNoSimulationsForCliente()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new SimulacaoRepository(context);
        var clienteId = Guid.NewGuid();

        // Act
        var result = await repository.GetByClienteIdAsync(clienteId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByClienteIdAsync_ShouldNotReturnSimulationsFromOtherClientes()
    {
        // Arrange
        using var context = CreateDbContext();
        var clienteId1 = Guid.NewGuid();
        var clienteId2 = Guid.NewGuid();

        await context.Simulacoes.AddRangeAsync(
            new Simulacao { Id = Guid.NewGuid(), ClienteId = clienteId1, ProdutoNome = "C1" },
            new Simulacao { Id = Guid.NewGuid(), ClienteId = clienteId2, ProdutoNome = "C2" }
        );
        await context.SaveChangesAsync();

        var repository = new SimulacaoRepository(context);

        // Act
        var result = await repository.GetByClienteIdAsync(clienteId1);

        // Assert
        result.Should().ContainSingle();
        result.First().ClienteId.Should().Be(clienteId1);
    }

    [Fact]
    public async Task GetAgregadoPorDiaAsync_ShouldReturnEmpty_WhenNoSimulations()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new SimulacaoRepository(context);

        // Act
        var result = await repository.GetAgregadoPorDiaAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAgregadoPorDiaAsync_ShouldGroupByProductAndDate()
    {
        // Arrange
        using var context = CreateDbContext();
        var hoje = DateTime.UtcNow.Date;
        var ontem = hoje.AddDays(-1);

        await context.Simulacoes.AddRangeAsync(
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ProdutoNome = "CDB",
                DataSimulacao = hoje,
                ValorFinal = 1000,
                ClienteId = Guid.NewGuid()
            },
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ProdutoNome = "CDB",
                DataSimulacao = hoje,
                ValorFinal = 2000,
                ClienteId = Guid.NewGuid()
            },
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ProdutoNome = "CDB",
                DataSimulacao = ontem,
                ValorFinal = 1500,
                ClienteId = Guid.NewGuid()
            }
        );
        await context.SaveChangesAsync();

        var repository = new SimulacaoRepository(context);

        // Act
        var result = (await repository.GetAgregadoPorDiaAsync()).ToList();

        // Assert
        result.Should().HaveCount(2); // 2 grupos: CDB/hoje e CDB/ontem
        
        var grupoHoje = result.FirstOrDefault(r => r.Data == hoje);
        grupoHoje.Should().NotBeNull();
        grupoHoje!.QuantidadeSimulacoes.Should().Be(2);
        grupoHoje.MediaValorFinal.Should().Be(1500); // (1000+2000)/2
    }

    [Fact]
    public async Task GetAgregadoPorDiaAsync_ShouldCalculateAverageCorrectly()
    {
        // Arrange
        using var context = CreateDbContext();
        var hoje = DateTime.UtcNow.Date;

        await context.Simulacoes.AddRangeAsync(
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ProdutoNome = "Fundo",
                DataSimulacao = hoje,
                ValorFinal = 100,
                ClienteId = Guid.NewGuid()
            },
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ProdutoNome = "Fundo",
                DataSimulacao = hoje,
                ValorFinal = 200,
                ClienteId = Guid.NewGuid()
            },
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ProdutoNome = "Fundo",
                DataSimulacao = hoje,
                ValorFinal = 300,
                ClienteId = Guid.NewGuid()
            }
        );
        await context.SaveChangesAsync();

        var repository = new SimulacaoRepository(context);

        // Act
        var result = (await repository.GetAgregadoPorDiaAsync()).ToList();

        // Assert
        result.Should().ContainSingle();
        result.First().QuantidadeSimulacoes.Should().Be(3);
        result.First().MediaValorFinal.Should().Be(200); // (100+200+300)/3
    }

    [Fact]
    public async Task GetAgregadoPorDiaAsync_ShouldIgnoreTimeComponent()
    {
        // Arrange
        using var context = CreateDbContext();
        var hoje = DateTime.UtcNow.Date;

        await context.Simulacoes.AddRangeAsync(
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ProdutoNome = "CDB",
                DataSimulacao = hoje.AddHours(8), // 8h da manhã
                ValorFinal = 1000,
                ClienteId = Guid.NewGuid()
            },
            new Simulacao
            {
                Id = Guid.NewGuid(),
                ProdutoNome = "CDB",
                DataSimulacao = hoje.AddHours(20), // 8h da noite
                ValorFinal = 2000,
                ClienteId = Guid.NewGuid()
            }
        );
        await context.SaveChangesAsync();

        var repository = new SimulacaoRepository(context);

        // Act
        var result = (await repository.GetAgregadoPorDiaAsync()).ToList();

        // Assert
        result.Should().ContainSingle(); // Deve agrupar no mesmo dia
        result.First().QuantidadeSimulacoes.Should().Be(2);
    }
}