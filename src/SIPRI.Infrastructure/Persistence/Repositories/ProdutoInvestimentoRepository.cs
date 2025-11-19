using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;
using SIPRI.Domain.Interfaces.Persistence;
using SIPRI.Infrastructure.Persistence.Contexts;

namespace SIPRI.Infrastructure.Persistence.Repositories;

public class ProdutoInvestimentoRepository : IProdutoInvestimentoRepository
{
    private readonly AppDbContext _context;

    public ProdutoInvestimentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProdutoInvestimento?> GetByIdAsync(Guid id)
    {
        return await _context.Produtos
            .FindAsync(id);
    }

    public async Task<IEnumerable<ProdutoInvestimento>> GetAllAsync()
    {
        return await _context.Produtos
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProdutoInvestimento>> GetByPerfilRiscoAsync(string perfilRisco)
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(p => p.Risco == perfilRisco || p.Risco == "Baixo")  
                                                                      
            .Where(p => p.Risco == perfilRisco)
            .ToListAsync();
    }

    public async Task<ProdutoInvestimento?> GetByTipoAsync(string tipoProduto)
    {
        // Busca o primeiro produto que corresponda ao tipo (ex: "CDB")
        return await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Tipo == tipoProduto);
    }
}