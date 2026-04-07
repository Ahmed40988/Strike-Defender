using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StrikeDefender.Domain.Rules;

namespace StrikeDefender.Infrastructure.Rules.Persistance;

public class RuleConfiguration : IEntityTypeConfiguration<WafRule>
{
    public void Configure(EntityTypeBuilder<WafRule> builder)
    {
        builder.ToTable("Rules");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RuleContent)
            .IsRequired()
            .HasMaxLength(4000);


        builder.HasIndex(x => x.RuleContent);
        builder.HasIndex(x => x.Createdon);
    }
}