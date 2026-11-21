namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>
/// Une séance de l'historique d'un titre. Cours : la clôture, à défaut le cours de référence
/// (en % du nominal pour une obligation, en FCFA pour une action). Volume : titres échangés, null s'il n'est pas publié.
/// </summary>
public sealed record PointCoursDto(DateOnly DateSeance, decimal Cours, long? Volume);
