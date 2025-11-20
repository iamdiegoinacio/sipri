using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;

namespace SIPRI.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        // Garante que o banco existe
        context.Database.EnsureCreated();

        // Se já tem produtos, não faz nada
        if (context.Produtos.Any())
        {
            return;
        }

        var produtos = new ProdutoInvestimento[]
        {
            new ProdutoInvestimento(Guid.NewGuid(), "CDB Caixa 2026", "CDB", 0.12m, "Baixo", 1),
            new ProdutoInvestimento(Guid.NewGuid(), "LCI CDI 90%", "LCI", 0.11m, "Baixo", 1),
            new ProdutoInvestimento(Guid.NewGuid(), "Fundo Ações Tech", "Fundo", 0.18m, "Alto", 3),
            new ProdutoInvestimento(Guid.NewGuid(), "Fundo Multimercado", "Fundo", 0.14m, "Moderado", 2)
        };

        context.Produtos.AddRange(produtos);
        context.SaveChanges();
    }
}