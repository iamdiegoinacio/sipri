using Microsoft.EntityFrameworkCore;
using SIPRI.Domain.Entities;

namespace SIPRI.Infrastructure.Persistence.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets representam as tabelas no SQL Server
    public DbSet<ProdutoInvestimento> Produtos { get; set; }
    public DbSet<Investimento> Investimentos { get; set; }
    public DbSet<Simulacao> Simulacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Scan automático das configurações Fluent API
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}