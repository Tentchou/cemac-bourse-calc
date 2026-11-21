namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>Un versement de dividende dans la liste de l'administration.</summary>
public sealed record DividendeAdminDto(
    Guid Id,
    Guid TitreId,
    string Isin,
    string Nom,
    int Exercice,
    decimal MontantBrut,
    decimal? MontantNet,
    DateOnly? DateDetachement,
    DateOnly? DatePaiement);
