namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>Le cours d'un titre pour une séance. Obligation : cours en % et contrôle du coupon couru publié.</summary>
public sealed record CoursAdminDto(
    Guid Id,
    Guid TitreId,
    string Isin,
    string Nom,
    string TypeTitre,
    DateOnly DateSeance,
    decimal? CoursReference,
    decimal? CoursCloture,
    long? Volume,
    decimal? CouponCouruPublie,
    ControleCouponCouruDto? Controle = null);
