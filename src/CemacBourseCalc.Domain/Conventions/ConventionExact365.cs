namespace CemacBourseCalc.Domain.Conventions;

/// <summary>Exact/365 : jours calendaires réels, divisés par 365 (même en année bissextile).</summary>
public sealed class ConventionExact365 : IConventionDecompte
{
    public string Code => "EXACT_365";
    public string Libelle => "Exact/365";
    public int JoursAnnee => 365;
    public int Jours(DateOnly debut, DateOnly fin) => fin.DayNumber - debut.DayNumber;
}
