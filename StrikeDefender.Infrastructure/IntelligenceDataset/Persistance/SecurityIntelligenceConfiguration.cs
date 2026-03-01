
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StrikeDefender.Domain.IntelligenceDataset;

namespace StrikeDefender.Infrastructure.IntelligenceDataset.Persistance;

public class SecurityIntelligenceConfiguration : IEntityTypeConfiguration<SecurityIntelligenceEntry>
{
    public void Configure(EntityTypeBuilder<SecurityIntelligenceEntry> builder)
    {
        builder.ToTable("SecurityIntelligenceEntries");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PayloadSnapshot)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(x => x.TargetSnapshot)
            .HasMaxLength(500);

        builder.Property(x => x.RuleSnapshot)
            .HasMaxLength(4000);

        builder.Property(x => x.AiAnalysis)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.SecurityAnalysis)
            .HasMaxLength(4000);

        builder.HasIndex(x => x.AttackType);
        builder.HasIndex(x => x.Severity);
        builder.HasIndex(x => x.Createdon);

        builder.HasOne(x => x.Attack)
            .WithMany()
            .HasForeignKey(x => x.AttackId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Rule)
            .WithMany()
            .HasForeignKey(x => x.RuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
