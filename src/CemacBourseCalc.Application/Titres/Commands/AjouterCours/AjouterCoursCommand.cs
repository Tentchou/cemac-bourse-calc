using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterCours;

/// <summary>
/// Une ligne du bulletin de cote pour une séance. Obligation : cours en % du nominal (pied de coupon) ;
/// action : cours en FCFA. Le coupon couru publié sert à contrôler notre calcul.
/// </summary>
public sealed record AjouterCoursCommand(
    Guid TitreId,
    DateOnly DateSeance,
    decimal? CoursReference,
    decimal? CoursCloture,
    long? Volume,
    decimal? CouponCouruPublie) : IRequest<Result<Guid>>;
