
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StrikeDefender.Domain.Attacks;

namespace StrikeDefender.Infrastructure.SuccessfulAttacks.Persistance;

public class SuccessfulAttackConfiguration : IEntityTypeConfiguration<SuccessfulAttack>
{
    public void Configure(EntityTypeBuilder<SuccessfulAttack> builder)
    {
        builder.ToTable("SuccessfulAttacks");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BypassTechnique)
            .HasMaxLength(2000);

        builder.Property(x => x.Notes)
            .HasMaxLength(4000);

        builder.HasIndex(x => x.Createdon);

        builder.HasOne(x => x.Attack)
            .WithMany()
            .HasForeignKey(x => x.AttackId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AttackResult)
            .WithMany()
            .HasForeignKey(x => x.AttackResultId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
