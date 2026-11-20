namespace CemacBourseCalc.Application.Calculs.Dtos;

/// <summary>Une échéance rendue à l'écran. Montants en FCFA, pour la quantité demandée.</summary>
public sealed record LigneEcheancierDto(
    int Rang,
    DateOnly DebutPeriode,
    DateOnly DateTheorique,
    DateOnly DatePaiement,
    bool PeriodeReguliere,
    decimal NominalDebutParTitre,
    decimal CouponParTitre,
    decimal CapitalParTitre,
    decimal NominalRestantApresParTitre,
    decimal Coupon,
    decimal Capital,
    decimal Total);

/// <summary>
/// L'échéancier d'une obligation. Si une date de négociation est fournie, seules les échéances
/// après le règlement de l'achat sont rendues (ce que l'acheteur percevra).
/// </summary>
public sealed record EcheancierDto(
    string Titre,
    string Convention,
    int Quantite,
    DateOnly? DateNegociation,
    DateOnly? DateReglement,
    IReadOnlyList<LigneEcheancierDto> Lignes,
    decimal TotalCoupons,
    decimal TotalCapital,
    decimal TotalFlux);
