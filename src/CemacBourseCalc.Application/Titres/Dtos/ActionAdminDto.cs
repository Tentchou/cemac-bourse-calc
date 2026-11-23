namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>Une action dans la liste de l'administration.</summary>
public sealed record ActionAdminDto(
    Guid Id,
    string Isin,
    string Nom,
    string Emetteur,
    string Pays,
    decimal Nominal,
    long NombreTitres,
    DateOnly? DateAdmission,
    decimal? DernierCours,
    DateOnly? DateDernierCours,
    string Statut);
