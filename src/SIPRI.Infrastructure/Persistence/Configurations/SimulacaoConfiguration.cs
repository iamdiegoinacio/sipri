using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIPRI.Domain.Entities;

namespace SIPRI.Infrastructure.Persistence.Configurations;

public class SimulacaoConfiguration : IEntityTypeConfiguration<Simulacao>
{
    public void Configure(EntityTypeBuilder<Simulacao> builder)
    {
        builder.ToTable("Simulacoes");

        builder.HasKey(s => s.Id);

        // Índices para performance nas consultas de histórico e agregação
        builder.HasIndex(s => s.ClienteId);
        builder.HasIndex(s => s.DataSimulacao); // Para relatórios por data

        builder.Property(s => s.ProdutoNome)
            .HasMaxLength(100)
            .IsRequired();

        // --- VALORES MONETÁRIOS ---
        builder.Property(s => s.ValorInvestido)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.ValorFinal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.PrazoMeses)
            .IsRequired();

        builder.Property(s => s.DataSimulacao)
            .IsRequired();
    }
}