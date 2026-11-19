using CemacBourseCalc.Domain.Titres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class DividendeConfiguration : IEntityTypeConfiguration<Dividende>
{
    public void Configure(EntityTypeBuilder<Dividende> b)
    {
        b.ToTable("dividende", t =>
        {
            t.HasCheckConstraint("ck_dividende_brut", "montant_brut > 0");
            t.HasCheckConstraint("ck_dividende_net", "montant_net IS NULL OR (montant_net > 0 AND montant_net <= montant_brut)");
            t.HasCheckConstraint("ck_dividende_dates", "date_paiement IS NULL OR date_detachement IS NULL OR date_paiement >= date_detachement");
        });
        b.HasKey(d => d.Id);
        b.Property(d => d.Id).ValueGeneratedNever();

        b.Property(d => d.MontantBrut).HasPrecision(18, 2);
        b.Property(d => d.MontantNet).HasPrecision(18, 2);

        b.HasOne<Titre>().WithMany().HasForeignKey(d => d.TitreId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(d => new { d.TitreId, d.Exercice }); // pas unique : acompte + solde sur un même exercice
    }
}
