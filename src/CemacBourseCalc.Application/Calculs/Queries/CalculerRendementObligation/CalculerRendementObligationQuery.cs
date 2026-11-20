using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Calculs.Queries.CalculerRendementObligation;

/// <summary>
/// Rendement courant et rendement actuariel d'une obligation achetée à un cours, pour une négociation
/// conclue à une date donnée (le calcul part du règlement, T + délai de la place).
/// </summary>
public sealed record CalculerRendementObligationQuery(Guid TitreId, DateOnly DateNegociation, decimal CoursPiedCoupon)
    : IRequest<Result<RendementObligationDto>>;
