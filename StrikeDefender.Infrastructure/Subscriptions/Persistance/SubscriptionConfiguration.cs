
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StrikeDefender.Domain.Subscriptions;

namespace StrikeDefender.Infrastructure.Subscriptions.Persistance;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.HasIndex(s => s.UserId).IsUnique(false);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.Createdon);

        builder.HasOne(x => x.Plan)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithOne(u => u.Subscription)
            .HasForeignKey<Subscription>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
