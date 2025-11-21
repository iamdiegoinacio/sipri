using SIPRI.Domain.Entities;
using SIPRI.Infrastructure.Persistence.Contexts;

namespace SIPRI.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        // Garante que o banco existe (Cria se não existir)
        context.Database.EnsureCreated();

        // Verifica se já existem produtos. Se sim, não faz nada (Seed é executado apenas uma vez)
        if (context.Produtos.Any())
        {
            return;
        }

        var produtos = new ProdutoInvestimento[]
        {
            // --- 1. Produtos Conservadores (Risco Baixo - Nível 1) ---
            new ProdutoInvestimento(Guid.NewGuid(), "CDB Caixa Fácil", "CDB", 0.1050m, "Baixo", 1),
            new ProdutoInvestimento(Guid.NewGuid(), "Tesouro Selic 2029", "Tesouro", 0.1075m, "Baixo", 1),
            new ProdutoInvestimento(Guid.NewGuid(), "LCI Habitacional 90 dias", "LCI", 0.0980m, "Baixo", 1),
            new ProdutoInvestimento(Guid.NewGuid(), "Fundo DI Referenciado", "Fundo", 0.1020m, "Baixo", 1),

            // --- 2. Produtos Moderados (Risco Moderado - Nível 2) ---
            new ProdutoInvestimento(Guid.NewGuid(), "CDB Pré-Fixado 2028", "CDB", 0.1250m, "Moderado", 2),
            new ProdutoInvestimento(Guid.NewGuid(), "Fundo Multimercado Dinâmico", "Fundo", 0.1400m, "Moderado", 2),
            new ProdutoInvestimento(Guid.NewGuid(), "Debênture Incentivada Infra", "Debenture", 0.1350m, "Moderado", 2),
            new ProdutoInvestimento(Guid.NewGuid(), "LCA Agronegócio 2 Anos", "LCA", 0.1150m, "Moderado", 2),

            // --- 3. Produtos Agressivos (Risco Alto - Nível 3) ---
            new ProdutoInvestimento(Guid.NewGuid(), "Fundo de Ações Ibovespa", "Fundo", 0.1800m, "Alto", 3),
            new ProdutoInvestimento(Guid.NewGuid(), "ETF Small Caps", "ETF", 0.2200m, "Alto", 3),
            new ProdutoInvestimento(Guid.NewGuid(), "COE Tecnologia Global", "COE", 0.2500m, "Alto", 3),
            // Rentabilidade base em dólar varia, mas risco é alto. Valor base simbólico:
            new ProdutoInvestimento(Guid.NewGuid(), "Fundo Cambial Dólar", "Fundo", 0.0500m, "Alto", 3)
        };

        context.Produtos.AddRange(produtos);
        context.SaveChanges();
    }
}