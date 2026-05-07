using Gold.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastructure.Persistence.Configurations;

public class RepairOrderConfiguration : IEntityTypeConfiguration<RepairOrder>
{
    public void Configure(EntityTypeBuilder<RepairOrder> builder)
    {
        builder.ToTable("RepairOrders");
        builder.HasKey(x => x.Id);
        builder.ConfigureBaseKey();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(40);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.ReceivingEmployeeName).IsRequired().HasMaxLength(150);
        builder.Property(x => x.WorkshopCourierName).HasMaxLength(150);
        builder.Property(x => x.ExternalProviderName).HasMaxLength(150);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.ImageBeforeUrl).HasMaxLength(500);
        builder.Property(x => x.ImageAfterUrl).HasMaxLength(500);

        builder.Property(x => x.WeightBefore).HasPrecision(10, 3);
        builder.Property(x => x.WeightAfter).HasPrecision(10, 3);
        builder.Property(x => x.Price).HasPrecision(18, 2);

        builder.Property(x => x.CustomerOtpCode).HasMaxLength(10);

        builder.HasOne(x => x.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Branch)
            .WithMany(b => b.Orders)
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Workshop)
            .WithMany(w => w.Orders)
            .HasForeignKey(x => x.WorkshopId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.StatusHistory)
            .WithOne(h => h.RepairOrder)
            .HasForeignKey(h => h.RepairOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("OrderStatusHistories");
        builder.HasKey(x => x.Id);
        builder.ConfigureBaseKey();
        builder.Property(x => x.Note).HasMaxLength(500);
        builder.Property(x => x.ActorUserName).HasMaxLength(150);
        builder.Property(x => x.ActorRole).HasMaxLength(40);
    }
}
