
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StrikeDefender.Domain.Attacks;

namespace StrikeDefender.Infrastructure.AttackResults.Persistance;

public class AttackResultConfiguration : IEntityTypeConfiguration<AttackResult>
{
    public void Configure(EntityTypeBuilder<AttackResult> builder)
    {
        builder.ToTable("AttackResults");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResponseBody).HasMaxLength(8000);

        builder.HasIndex(x => x.IsSuccessful);
        builder.HasIndex(x => x.Createdon);

        builder.HasOne(x => x.Attack)
            .WithMany()
            .HasForeignKey(x => x.AttackId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
