namespace CemacBourseCalc.Application.Calculs.Dtos;

/// <summary>Un flux futur d'un titre et sa valeur actualisée, pour l'explication du rendement actuariel.</summary>
public sealed record FluxActualiseDto(
    DateOnly Date,
    int Jours,
    decimal FractionAnnee,
    decimal Coupon,
    decimal Capital,
    decimal Flux,
    decimal FacteurActualisation,
    decimal ValeurActuelle);

/// <summary>Rendements d'une obligation pour un titre, en %, avec chaque étape du calcul. Montants en FCFA.</summary>
public sealed record RendementObligationDto(
    string Titre,
    DateOnly DateNegociation,
    int DelaiReglementJoursOuvres,
    DateOnly DateReglement,
    decimal CoursPiedCoupon,
    decimal NominalRestant,
    decimal TauxFacial,
    decimal PrixPiedCouponParTitre,
    decimal CouponCouruParTitre,
    decimal PrixPleinCouponParTitre,
    decimal CouponAnnuelParTitre,
    decimal RendementCourant,
    decimal RendementActuariel,
    int JoursAnneeActuarielle,
    IReadOnlyList<FluxActualiseDto> Flux);
