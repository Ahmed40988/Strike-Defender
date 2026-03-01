
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StrikeDefender.Domain.Attacks;

namespace StrikeDefender.Infrastructure.Attacks.Persistance
{
    public class AttackConfiguration : IEntityTypeConfiguration<Attack>
    {
        public void Configure(EntityTypeBuilder<Attack> builder)
        {
            builder.ToTable("Attacks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Payload)
                   .IsRequired()
                   .HasMaxLength(5000);

            builder.Property(x => x.Target)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(x => x.Rule)
                   .WithMany(r => r.Attacks)
                   .HasForeignKey(x => x.RuleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
