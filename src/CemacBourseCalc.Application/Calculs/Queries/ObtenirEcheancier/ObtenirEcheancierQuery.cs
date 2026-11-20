using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Calculs.Queries.ObtenirEcheancier;

/// <summary>Les coupons et remboursements d'une obligation, pour une quantité ; après un achat si une date de négociation est donnée.</summary>
public sealed record ObtenirEcheancierQuery(Guid TitreId, int Quantite, DateOnly? DateNegociation)
    : IRequest<Result<EcheancierDto>>;
