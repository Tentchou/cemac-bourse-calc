using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Titres.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.SupprimerTitre;

/// <summary>
/// Retire un titre. Sans aucun historique (erreur de saisie), il est effacé ; avec des cours,
/// des dividendes ou des documents, il est radié : il disparaît du site public, son historique reste.
/// </summary>
public sealed record SupprimerTitreCommand(Guid Id) : IRequest<Result<SuppressionTitreDto>>;
