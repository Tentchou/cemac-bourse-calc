using CemacBourseCalc.Domain.Administration;
using CemacBourseCalc.Domain.Communs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class AdministrateurConfiguration : IEntityTypeConfiguration<Administrateur>
{
    public void Configure(EntityTypeBuilder<Administrateur> b)
    {
        b.ToTable("administrateur", t =>
        {
            t.HasCheckConstraint("ck_administrateur_role", Contraintes.ValeursEnum<RoleAdministrateur>("role"));
            t.HasCheckConstraint("ck_administrateur_email_minuscules", "email = lower(email)");
        });
        b.HasKey(a => a.Id);
        b.Property(a => a.Id).ValueGeneratedNever();

        b.Property(a => a.Email).HasMaxLength(254).IsRequired();
        b.HasIndex(a => a.Email).IsUnique(); // stocké en minuscules : l'unicité ignore donc la casse
        b.Property(a => a.Nom).HasMaxLength(200).IsRequired();
        b.Property(a => a.MotDePasseHash).HasMaxLength(500).IsRequired();
        b.Property(a => a.Role).HasConversion<string>().HasMaxLength(20);
    }
}
