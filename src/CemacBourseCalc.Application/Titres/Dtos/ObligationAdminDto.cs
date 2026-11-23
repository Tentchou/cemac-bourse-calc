namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>Une obligation dans la liste de l'administration, avec le contrôle de son dernier coupon couru publié.</summary>
public sealed record ObligationAdminDto(
    Guid Id,
    string Isin,
    string Nom,
    string Emetteur,
    string Pays,
    decimal Nominal,
    decimal TauxFacial,
    string Periodicite,
    string ModeRemboursement,
    int DiffereEcheances,
    DateOnly DateJouissance,
    DateOnly DateEcheance,
    string CodeConvention,
    string RegimeFiscal,
    string Statut,
    ControleCouponCouruDto? Controle = null);
