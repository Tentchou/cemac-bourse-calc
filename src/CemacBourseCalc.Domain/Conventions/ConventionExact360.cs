namespace CemacBourseCalc.Domain.Conventions;

/// <summary>Exact/360 : jours calendaires réels, divisés par 360.</summary>
public sealed class ConventionExact360 : IConventionDecompte
{
    public string Code => "EXACT_360";
    public string Libelle => "Exact/360";
    public int JoursAnnee => 360;
    public int Jours(DateOnly debut, DateOnly fin) => fin.DayNumber - debut.DayNumber;
}
