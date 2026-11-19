using CemacBourseCalc.Domain.Titres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

/// <summary>
/// Une seule table « titre » pour les obligations et les actions (colonne type_titre).
/// Montants en numeric(18,2), taux en numeric(9,6) : jamais de float pour de l'argent.
/// </summary>
public sealed class TitreConfiguration : IEntityTypeConfiguration<Titre>
{
    public const string TypeObligation = "OBLIGATION";
    public const string TypeAction = "ACTION";

    public void Configure(EntityTypeBuilder<Titre> b)
    {
        b.ToTable("titre");
        b.HasKey(t => t.Id);

        b.HasDiscriminator<string>("type_titre")
            .HasValue<Obligation>(TypeObligation)
            .HasValue<Domain.Titres.Action>(TypeAction);
        b.Property<string>("type_titre").HasMaxLength(20);

        b.Property(t => t.Isin)
            .HasConversion(i => i.Valeur, valeur => Isin.Creer(valeur))
            .HasMaxLength(12)
            .IsRequired();
        b.HasIndex(t => t.Isin).IsUnique();

        b.Property(t => t.Nom).HasMaxLength(200).IsRequired();
        b.Property(t => t.Emetteur).HasMaxLength(200).IsRequired();
        b.Property(t => t.Nominal).HasPrecision(18, 2);

        b.Ignore(t => t.Pays); // lu sur l'ISIN
    }
}
