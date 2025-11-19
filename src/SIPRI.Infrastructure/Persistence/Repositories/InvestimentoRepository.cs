using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Infrastructure.Persistence.Contexts;

namespace SIPRI.Infrastructure.Persistence.Repositories;

public class InvestimentoRepository : IInvestimentoRepository
{
    private readonly AppDbContext _context;

    public InvestimentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Investimento>> GetByClienteIdAsync(Guid clienteId)
    {
        return await _context.Investimentos
            .AsNoTracking()
            .Where(i => i.ClienteId == clienteId)
            .OrderByDescending(i => i.Data) // Traz os mais recentes primeiro
            .ToListAsync();
    }
}