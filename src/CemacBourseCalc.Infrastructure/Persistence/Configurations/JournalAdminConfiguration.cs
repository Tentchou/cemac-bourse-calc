using CemacBourseCalc.Domain.Administration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class JournalAdminConfiguration : IEntityTypeConfiguration<JournalAdmin>
{
    public void Configure(EntityTypeBuilder<JournalAdmin> b)
    {
        b.ToTable("journal_admin");
        b.HasKey(j => j.Id);
        b.Property(j => j.Id).ValueGeneratedNever();

        b.Property(j => j.Action).HasMaxLength(60).IsRequired();
        b.Property(j => j.Entite).HasMaxLength(60).IsRequired();
        b.Property(j => j.EntiteId).HasMaxLength(60).IsRequired();
        b.Property(j => j.Avant).HasColumnType("jsonb");
        b.Property(j => j.Apres).HasColumnType("jsonb");

        b.HasOne<Administrateur>().WithMany().HasForeignKey(j => j.AdministrateurId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(j => new { j.Entite, j.EntiteId }); // l'historique d'un titre
        b.HasIndex(j => j.Date);
    }
}
