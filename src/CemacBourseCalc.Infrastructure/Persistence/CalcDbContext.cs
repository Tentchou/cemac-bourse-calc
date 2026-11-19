using CemacBourseCalc.Domain.Titres;
using Microsoft.EntityFrameworkCore;

namespace CemacBourseCalc.Infrastructure.Persistence;

/// <summary>Contexte EF Core : il ne sert qu'aux écritures (administration). Les lectures passent par Dapper.</summary>
public sealed class CalcDbContext(DbContextOptions<CalcDbContext> options) : DbContext(options)
{
    public DbSet<Titre> Titres => Set<Titre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CalcDbContext).Assembly);
}
