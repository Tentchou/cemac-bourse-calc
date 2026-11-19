using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

public sealed class DocumentSourceConfiguration : IEntityTypeConfiguration<DocumentSource>
{
    public void Configure(EntityTypeBuilder<DocumentSource> b)
    {
        b.ToTable("document_source", t => t.HasCheckConstraint("ck_document_source_type", Contraintes.ValeursEnum<TypeDocument>("type")));
        b.HasKey(d => d.Id);
        b.Property(d => d.Id).ValueGeneratedNever();

        b.Property(d => d.Type).HasConversion<string>().HasMaxLength(20);
        b.Property(d => d.Libelle).HasMaxLength(300).IsRequired();
        b.Property(d => d.Url).HasMaxLength(500).IsRequired();

        b.HasOne<Titre>().WithMany().HasForeignKey(d => d.TitreId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(d => d.TitreId);
    }
}
