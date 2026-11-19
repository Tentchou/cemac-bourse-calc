using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Referentiel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class PaysConfiguration : IEntityTypeConfiguration<Pays>
{
    public void Configure(EntityTypeBuilder<Pays> b)
    {
        b.ToTable("pays", t => t.HasCheckConstraint("ck_pays_code", Contraintes.ValeursEnum<PaysCemac>("code")));
        b.HasKey(p => p.Code);
        b.Property(p => p.Code).HasConversion<string>().HasMaxLength(2);
        b.Property(p => p.Libelle).HasMaxLength(100).IsRequired();

        // Les six pays de la CEMAC, créés par la migration.
        b.HasData(Enum.GetValues<PaysCemac>().Select(code => new Pays(code)));
    }
}
