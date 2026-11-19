using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Referentiel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class EmetteurConfiguration : IEntityTypeConfiguration<Emetteur>
{
    public void Configure(EntityTypeBuilder<Emetteur> b)
    {
        b.ToTable("emetteur", t => t.HasCheckConstraint("ck_emetteur_type", Contraintes.ValeursEnum<TypeEmetteur>("type")));
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).ValueGeneratedNever();

        b.Property(e => e.Nom).HasMaxLength(200).IsRequired();
        b.HasIndex(e => e.Nom).IsUnique();
        b.Property(e => e.Sigle).HasMaxLength(30);
        b.Property(e => e.Type).HasConversion<string>().HasMaxLength(20);
        b.Property(e => e.PaysCode).HasConversion<string>().HasMaxLength(2);
        b.Property(e => e.SiteWeb).HasMaxLength(300);

        b.HasOne<Pays>().WithMany().HasForeignKey(e => e.PaysCode).OnDelete(DeleteBehavior.Restrict);
    }
}
