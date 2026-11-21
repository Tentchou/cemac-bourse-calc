namespace CemacBourseCalc.Application.Bulletins.Dtos;

/// <summary>Ce que le bulletin de la cote dit d'un titre pour la séance.</summary>
public sealed record CoursLu(
    int Page,
    string Isin,
    string Libelle,
    string Section,
    decimal? CoursCloture,
    long? Volume,
    decimal? CouponCouruPublie);

/// <summary>
/// Le bulletin lu : son numéro, la date de la séance, les cours lus et les lignes illisibles
/// (une ligne illisible bloque l'import : on n'enregistre jamais une valeur devinée).
/// </summary>
public sealed record BulletinLu(int Numero, DateOnly DateSeance, IReadOnlyList<CoursLu> Cours, IReadOnlyList<string> Anomalies);
