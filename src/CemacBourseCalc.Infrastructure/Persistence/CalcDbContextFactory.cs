using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CemacBourseCalc.Infrastructure.Persistence;

/// <summary>
/// Utilisé uniquement par « dotnet ef migrations add » : générer une migration ne
/// se connecte pas à la base, la chaîne ci-dessous n'est jamais ouverte.
/// Pour appliquer les migrations : dotnet ef database update --connection "…".
/// </summary>
public sealed class CalcDbContextFactory : IDesignTimeDbContextFactory<CalcDbContext>
{
    public CalcDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<CalcDbContext>()
            .UseNpgsql("Host=localhost;Database=cemac_bourse_calc")
            .UseSnakeCaseNamingConvention()
            .Options);
}
