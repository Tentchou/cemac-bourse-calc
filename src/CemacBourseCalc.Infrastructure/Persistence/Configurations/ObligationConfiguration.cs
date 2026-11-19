using CemacBourseCalc.Domain.Titres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class ObligationConfiguration : IEntityTypeConfiguration<Obligation>
{
    public void Configure(EntityTypeBuilder<Obligation> b)
    {
        b.Property(o => o.TauxFacial).HasPrecision(9, 6);
        b.Property(o => o.Periodicite).HasConversion<string>().HasMaxLength(20);
        b.Property(o => o.ModeRemboursement).HasConversion<string>().HasMaxLength(30);
        b.Property(o => o.CodeConvention).HasMaxLength(20);
        b.Ignore(o => o.Convention); // retrouvée par son code
    }
}
