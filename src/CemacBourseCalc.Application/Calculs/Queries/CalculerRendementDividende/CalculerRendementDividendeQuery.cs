using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Calculs.Queries.CalculerRendementDividende;

/// <summary>Rendement du dividende d'une action à un cours donné (FCFA), sur son dernier exercice saisi.</summary>
public sealed record CalculerRendementDividendeQuery(Guid TitreId, decimal Cours) : IRequest<Result<RendementDividendeDto>>;
