using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class ObligationConfiguration : IEntityTypeConfiguration<Obligation>
{
    public void Configure(EntityTypeBuilder<Obligation> b)
    {
        b.ToTable("obligation", t =>
        {
            t.Property(o => o.Id).HasColumnName("titre_id");
            t.HasCheckConstraint("ck_obligation_periodicite", Contraintes.ValeursEnum<Periodicite>("periodicite"));
            t.HasCheckConstraint("ck_obligation_mode_remboursement", Contraintes.ValeursEnum<ModeRemboursement>("mode_remboursement"));
            t.HasCheckConstraint("ck_obligation_code_convention", Contraintes.Conventions("code_convention"));
            t.HasCheckConstraint("ck_obligation_regime_fiscal", Contraintes.ValeursEnum<RegimeFiscal>("regime_fiscal"));
            t.HasCheckConstraint("ck_obligation_taux_facial", "taux_facial >= 0 AND taux_facial <= 100");
            t.HasCheckConstraint("ck_obligation_dates", "date_echeance > date_jouissance");
            t.HasCheckConstraint("ck_obligation_premier_coupon",
                "date_premier_coupon IS NULL OR (date_premier_coupon > date_jouissance AND date_premier_coupon <= date_echeance)");
            t.HasCheckConstraint("ck_obligation_differe", "differe_echeances >= 0");
        });

        b.Property(o => o.TauxFacial).HasPrecision(9, 6);
        b.Property(o => o.Periodicite).HasConversion<string>().HasMaxLength(20);
        b.Property(o => o.ModeRemboursement).HasConversion<string>().HasMaxLength(30);
        b.Property(o => o.CodeConvention).HasMaxLength(20).IsRequired();
        b.Property(o => o.RegimeFiscal).HasConversion<string>().HasMaxLength(10);
        b.Ignore(o => o.Convention); // retrouvée par son code
    }
}
