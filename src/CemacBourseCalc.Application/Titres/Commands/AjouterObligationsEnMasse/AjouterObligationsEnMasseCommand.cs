using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Titres.Commands.AjouterObligation;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterObligationsEnMasse;

/// <summary>Plusieurs obligations d'un coup : chaque ligne est une saisie manuelle (<see cref="AjouterObligationCommand"/>).</summary>
public sealed record AjouterObligationsEnMasseCommand(IReadOnlyList<AjouterObligationCommand> Lignes)
    : IRequest<Result<ResultatMasseDto>>;
