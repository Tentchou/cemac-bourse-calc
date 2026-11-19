using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Referentiel;
using CemacBourseCalc.Domain.Titres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

/// <summary>
/// Héritage en tables séparées (TPT) : « titre » porte le commun, « obligation » et « action »
/// leurs colonnes propres, avec titre_id comme clé et clé étrangère. Aucune colonne vide d'un côté.
/// Montants en numeric(18,2), jamais de float pour de l'argent.
/// </summary>
public sealed class TitreConfiguration : IEntityTypeConfiguration<Titre>
{
    public void Configure(EntityTypeBuilder<Titre> b)
    {
        b.UseTptMappingStrategy();
        b.ToTable("titre", t =>
        {
            t.HasCheckConstraint("ck_titre_type_titre", Contraintes.ValeursEnum<TypeTitre>("type_titre"));
            t.HasCheckConstraint("ck_titre_statut", Contraintes.ValeursEnum<StatutTitre>("statut"));
            t.HasCheckConstraint("ck_titre_nominal", "nominal > 0");
        });

        // Les clés primaires des trois tables TPT gardent le nom donné par EF Core (PK_titre,
        // PK_obligation, PK_action) : un nom posé ici serait recopié sur les trois tables, et
        // PostgreSQL refuse deux contraintes du même nom.
        b.HasKey(t => t.Id);
        b.Property(t => t.Id).ValueGeneratedNever();

        b.Property(t => t.Isin)
            .HasConversion(i => i.Valeur, valeur => Isin.Creer(valeur))
            .HasMaxLength(12)
            .IsRequired();
        b.HasIndex(t => t.Isin).IsUnique();

        b.Property(t => t.Nom).HasMaxLength(200).IsRequired();
        b.Property(t => t.TypeTitre).HasConversion<string>().HasMaxLength(20);
        b.Property(t => t.Statut).HasConversion<string>().HasMaxLength(20);
        b.Property(t => t.Nominal).HasPrecision(18, 2);

        b.HasOne<Emetteur>().WithMany().HasForeignKey(t => t.EmetteurId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(t => t.EmetteurId);
    }
}
