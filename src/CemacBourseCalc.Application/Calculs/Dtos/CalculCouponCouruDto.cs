namespace CemacBourseCalc.Application.Calculs.Dtos;

/// <summary>
/// Le résultat rendu à l'écran, avec chaque étape pour l'explication. Les montants sont en FCFA.
/// Les champs du prix ne sont renseignés que si un cours a été fourni.
/// </summary>
public sealed record CalculCouponCouruDto(
    string Titre,
    DateOnly DateNegociation,
    int DelaiReglementJoursOuvres,
    DateOnly DateReglement,
    DateOnly DebutPeriode,
    DateOnly FinPeriode,
    string Convention,
    int Jours,
    int JoursAnnee,
    decimal NominalRestant,
    decimal TauxFacial,
    int Quantite,
    decimal CouponCouruParTitre,
    decimal CouponCouruTotal,
    decimal? CoursPiedCoupon,
    decimal? PrixPiedCouponParTitre,
    decimal? PrixPiedCouponTotal,
    decimal? PrixPleinCouponTotal);
