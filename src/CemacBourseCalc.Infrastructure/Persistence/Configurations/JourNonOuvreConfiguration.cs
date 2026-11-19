using CemacBourseCalc.Domain.Place;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

/// <summary>Un seul calendrier : celui de la place BVMAC.</summary>
public sealed class JourNonOuvreConfiguration : IEntityTypeConfiguration<JourNonOuvre>
{
    public void Configure(EntityTypeBuilder<JourNonOuvre> b)
    {
        b.ToTable("jour_non_ouvre");
        b.HasKey(j => j.Date);
        b.Property(j => j.Libelle).HasMaxLength(100).IsRequired();
    }
}
