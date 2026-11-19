using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ActionCotee = CemacBourseCalc.Domain.Titres.Action;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class ActionConfiguration : IEntityTypeConfiguration<ActionCotee>
{
    public void Configure(EntityTypeBuilder<ActionCotee> b)
    {
        b.ToTable("action", t =>
        {
            t.Property(a => a.Id).HasColumnName("titre_id");
            t.HasCheckConstraint("ck_action_nombre_titres", "nombre_titres > 0");
        });
    }
}
