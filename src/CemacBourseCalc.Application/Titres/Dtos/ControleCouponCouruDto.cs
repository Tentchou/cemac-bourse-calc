namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>
/// Notre coupon couru comparé à celui publié par la BVMAC pour une séance. Calculé à la date de règlement
/// de la séance (délai de la place, jours non ouvrés compris). Erreur : le calcul n'a pas pu se faire.
/// </summary>
public sealed record ControleCouponCouruDto(
    DateOnly DateSeance,
    decimal Publie,
    DateOnly? DateReglement,
    decimal? Calcule,
    decimal? Ecart,
    bool Identique,
    string? Erreur);
