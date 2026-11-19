using CemacBourseCalc.Domain.Administration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class JetonSessionConfiguration : IEntityTypeConfiguration<JetonSession>
{
    public void Configure(EntityTypeBuilder<JetonSession> b)
    {
        b.ToTable("jeton_session", t => t.HasCheckConstraint("ck_jeton_session_expiration", "expire_le > cree_le"));
        b.HasKey(j => j.Id);
        b.Property(j => j.Id).ValueGeneratedNever();

        b.Property(j => j.JetonHash).HasMaxLength(128).IsRequired();
        b.HasIndex(j => j.JetonHash).IsUnique();

        b.HasOne<Administrateur>().WithMany().HasForeignKey(j => j.AdministrateurId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(j => j.AdministrateurId);
    }
}
