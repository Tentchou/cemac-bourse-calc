namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>
/// Une ligne de la liste publique des titres. Taux et échéance : obligations seulement.
/// Dernier cours : en % du nominal pour une obligation, en FCFA pour une action ; null si aucun cours saisi.
/// </summary>
public sealed record TitreResumeDto(
    Guid Id,
    string Isin,
    string Nom,
    string Emetteur,
    string Pays,
    string TypeTitre,
    decimal Nominal,
    string Statut,
    decimal? TauxFacial,
    DateOnly? DateEcheance,
    decimal? DernierCours,
    DateOnly? DateDernierCours);
