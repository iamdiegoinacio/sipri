using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIPRI.Domain.Entities;

namespace SIPRI.Infrastructure.Persistence.Configurations;

public class ProdutoInvestimentoConfiguration : IEntityTypeConfiguration<ProdutoInvestimento>
{
    public void Configure(EntityTypeBuilder<ProdutoInvestimento> builder)
    {
        builder.ToTable("ProdutosInvestimento");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Tipo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Risco)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.RentabilidadeBase)
            .HasPrecision(10, 4)
            .IsRequired();

        builder.Property(p => p.NivelRisco)
            .IsRequired();
    }
}