using CemacBourseCalc.Domain.Administration;
using CemacBourseCalc.Domain.Place;
using CemacBourseCalc.Domain.Referentiel;
using CemacBourseCalc.Domain.Titres;
using Microsoft.EntityFrameworkCore;

namespace CemacBourseCalc.Infrastructure.Persistence;

/// <summary>Contexte EF Core : il ne sert qu'aux écritures (administration). Les lectures passent par Dapper.</summary>
public sealed class CalcDbContext(DbContextOptions<CalcDbContext> options) : DbContext(options)
{
    public DbSet<Pays> Pays => Set<Pays>();
    public DbSet<Emetteur> Emetteurs => Set<Emetteur>();
    public DbSet<Titre> Titres => Set<Titre>();
    public DbSet<Cours> Cours => Set<Cours>();
    public DbSet<Dividende> Dividendes => Set<Dividende>();
    public DbSet<DocumentSource> DocumentsSource => Set<DocumentSource>();
    public DbSet<JourNonOuvre> JoursNonOuvres => Set<JourNonOuvre>();
    public DbSet<Parametre> Parametres => Set<Parametre>();
    public DbSet<Administrateur> Administrateurs => Set<Administrateur>();
    public DbSet<JetonSession> JetonsSession => Set<JetonSession>();
    public DbSet<JournalAdmin> JournalAdmin => Set<JournalAdmin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CalcDbContext).Assembly);
}
