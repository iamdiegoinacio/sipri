using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIPRI.Domain.Entities;

namespace SIPRI.Infrastructure.Persistence.Configurations;

public class InvestimentoConfiguration : IEntityTypeConfiguration<Investimento>
{
    public void Configure(EntityTypeBuilder<Investimento> builder)
    {
        builder.ToTable("Investimentos");

        builder.HasKey(i => i.Id);

        // Como o ClienteId é apenas lógico, indexado para performance.
        builder.HasIndex(i => i.ClienteId);

        builder.Property(i => i.Tipo)
            .HasMaxLength(50)
            .IsRequired();

        // --- VALORES MONETÁRIOS ---
        builder.Property(i => i.Valor)
            .HasPrecision(18, 2)
            .IsRequired();

        // Rentabilidade efetiva (taxa)
        builder.Property(i => i.Rentabilidade)
            .HasPrecision(10, 4)
            .IsRequired();

        builder.Property(i => i.Data)
            .IsRequired();
    }
}