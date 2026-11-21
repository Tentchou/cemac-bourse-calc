using CemacBourseCalc.Domain.Titres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class CoursConfiguration : IEntityTypeConfiguration<Cours>
{
    public void Configure(EntityTypeBuilder<Cours> b)
    {
        b.ToTable("cours", t =>
        {
            t.HasCheckConstraint("ck_cours_un_cours", "cours_reference IS NOT NULL OR cours_cloture IS NOT NULL");
            t.HasCheckConstraint("ck_cours_positifs", "(cours_reference IS NULL OR cours_reference > 0) AND (cours_cloture IS NULL OR cours_cloture > 0)");
            t.HasCheckConstraint("ck_cours_volume", "volume IS NULL OR volume >= 0");
        });
        b.HasKey(c => c.Id);
        b.Property(c => c.Id).ValueGeneratedNever();

        // Cours en % (obligations, 4 décimales) ou en FCFA (actions) : numeric(18,4) couvre les deux.
        b.Property(c => c.CoursReference).HasPrecision(18, 4);
        b.Property(c => c.CoursCloture).HasPrecision(18, 4);
        b.Property(c => c.CouponCouruPublie).HasPrecision(18, 2);

        b.HasOne<Titre>().WithMany().HasForeignKey(c => c.TitreId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(c => new { c.TitreId, c.DateSeance }).IsUnique(); // une ligne par titre et par séance
        b.HasIndex(c => c.DateSeance); // liste d'une séance, dernière séance, liste des séances (administration)
    }
}
