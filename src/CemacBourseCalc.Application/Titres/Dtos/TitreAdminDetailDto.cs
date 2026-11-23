namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>
/// Un titre tel que le formulaire de modification l'attend : mêmes noms de champs que la saisie.
/// Les champs d'obligation sont vides pour une action, NombreTitres l'est pour une obligation.
/// NombreCours, NombreDividendes et NombreDocuments disent si le titre a un historique
/// (il est alors radié, jamais effacé).
/// </summary>
public sealed record TitreAdminDetailDto(
    Guid Id,
    string TypeTitre,
    string Isin,
    string Nom,
    Guid EmetteurId,
    decimal Nominal,
    DateOnly? DateAdmission,
    string Statut,
    decimal? TauxFacial,
    string? Periodicite,
    DateOnly? DateJouissance,
    DateOnly? DatePremierCoupon,
    DateOnly? DateEcheance,
    string? ModeRemboursement,
    int? DiffereEcheances,
    string? CodeConvention,
    string? RegimeFiscal,
    long? NombreTitres,
    int NombreCours,
    int NombreDividendes,
    int NombreDocuments);
