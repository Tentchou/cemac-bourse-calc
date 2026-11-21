using CemacBourseCalc.Domain.Calculs;

namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>Ce que la lecture rend pour une obligation : la ligne, ses caractéristiques de calcul et son dernier coupon couru publié.</summary>
public sealed record ObligationAdminLigne(
    ObligationAdminDto Obligation,
    CaracteristiquesObligation Caracteristiques,
    DateOnly? DateSeance,
    decimal? CouponPublie);
