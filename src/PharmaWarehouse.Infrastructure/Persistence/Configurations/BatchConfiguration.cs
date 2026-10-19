using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Infrastructure.Persistence.Configurations;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.Property(b => b.BatchNumber).IsRequired().HasMaxLength(50);
        builder.Property(b => b.PurchasePrice).HasPrecision(18, 2);

        builder.HasIndex(b => new { b.DrugId, b.BatchNumber }).IsUnique();
    }
}