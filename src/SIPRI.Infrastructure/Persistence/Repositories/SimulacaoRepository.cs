using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Domain.Models;
using SIPRI.Infrastructure.Persistence.Contexts;

namespace SIPRI.Infrastructure.Persistence.Repositories;

public class SimulacaoRepository : ISimulacaoRepository
{
    private readonly AppDbContext _context;

    public SimulacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Simulacao simulacao)
    {
        await _context.Simulacoes.AddAsync(simulacao);
    }

    public async Task<IEnumerable<Simulacao>> GetAllAsync()
    {
        return await _context.Simulacoes
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Simulacao>> GetByClienteIdAsync(Guid clienteId)
    {
        return await _context.Simulacoes
            .AsNoTracking()
            .Where(s => s.ClienteId == clienteId)
            .OrderByDescending(s => s.DataSimulacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<SimulacaoAgregada>> GetAgregadoPorDiaAsync()
    {
        var query = _context.Simulacoes
            .AsNoTracking()
            .GroupBy(s => new { s.ProdutoNome, Data = s.DataSimulacao.Date }) // Agrupa por Nome e Data (sem hora)
            .Select(g => new SimulacaoAgregada
            {
                Produto = g.Key.ProdutoNome,
                Data = g.Key.Data,
                QuantidadeSimulacoes = g.Count(),
                MediaValorFinal = g.Average(s => s.ValorFinal)
            });

        return await query.ToListAsync();
    }
}