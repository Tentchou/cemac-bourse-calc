namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>
/// La fiche publique d'un titre. Exactement un des deux blocs est renseigné, selon le type.
/// Dernier cours : en % du nominal pour une obligation, en FCFA pour une action.
/// </summary>
public sealed record FicheTitreDto(
    Guid Id,
    string Isin,
    string Nom,
    string Emetteur,
    string Pays,
    string TypeTitre,
    decimal Nominal,
    string Statut,
    decimal? DernierCours,
    DateOnly? DateDernierCours,
    FicheObligationDto? Obligation,
    FicheActionDto? Action);

/// <summary>Les caractéristiques d'une obligation. Convention : son libellé (« Exact/365 »).</summary>
public sealed record FicheObligationDto(
    decimal TauxFacial,
    string Periodicite,
    string ModeRemboursement,
    DateOnly DateJouissance,
    DateOnly? DatePremierCoupon,
    DateOnly DateEcheance,
    string Convention,
    string RegimeFiscal);

/// <summary>Une action et ses dividendes, du plus récent au plus ancien.</summary>
public sealed record FicheActionDto(long NombreTitres, IReadOnlyList<DividendeDto> Dividendes);

/// <summary>Un versement de dividende, par action, en FCFA.</summary>
public sealed record DividendeDto(int Exercice, decimal Brut, decimal? Net, DateOnly? DateDetachement, DateOnly? DatePaiement);
