using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Calculs.Queries.CalculerCouponCouru;

/// <summary>
/// Coupon couru d'une obligation pour une négociation conclue à une date donnée. Le coupon court
/// jusqu'au règlement (T + délai de la place). Avec un cours, le prix plein coupon est calculé aussi.
/// </summary>
public sealed record CalculerCouponCouruQuery(Guid TitreId, DateOnly DateNegociation, int Quantite, decimal? CoursPiedCoupon)
    : IRequest<Result<CalculCouponCouruDto>>;
