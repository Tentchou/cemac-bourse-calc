namespace CemacBourseCalc.Domain.Communs;

/// <summary>
/// Les identifiants sont des GUID version 7 : ils sont créés par le Domain, sans
/// aller-retour en base, et ordonnés dans le temps, ce qui garde les index compacts.
/// </summary>
public static class Identifiant
{
    public static Guid Nouveau() => Guid.CreateVersion7();
}

/// <summary>Les dates de saisie sont toujours enregistrées en UTC (colonnes timestamptz).</summary>
public static class Horodatage
{
    public static DateTimeOffset EnUtc(DateTimeOffset instant) => instant.ToUniversalTime();
}
