using CemacBourseCalc.Domain.Administration;
using CemacBourseCalc.Domain.Place;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class ParametreConfiguration : IEntityTypeConfiguration<Parametre>
{
    public void Configure(EntityTypeBuilder<Parametre> b)
    {
        b.ToTable("parametre");
        b.HasKey(p => p.Cle);
        b.Property(p => p.Cle).HasMaxLength(60);
        b.Property(p => p.Valeur).HasMaxLength(200).IsRequired();
        b.Property(p => p.Libelle).HasMaxLength(300).IsRequired();

        b.HasOne<Administrateur>().WithMany().HasForeignKey(p => p.ModifieParId).OnDelete(DeleteBehavior.Restrict);

        // Valeurs initiales, modifiables ensuite par l'administration.
        b.HasData(ClesParametres.ValeursInitiales);
    }
}
