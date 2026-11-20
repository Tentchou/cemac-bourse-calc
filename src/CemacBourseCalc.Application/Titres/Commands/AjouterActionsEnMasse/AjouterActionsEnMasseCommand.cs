using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Titres.Commands.AjouterAction;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterActionsEnMasse;

/// <summary>Plusieurs actions d'un coup : chaque ligne est une saisie manuelle (<see cref="AjouterActionCommand"/>).</summary>
public sealed record AjouterActionsEnMasseCommand(IReadOnlyList<AjouterActionCommand> Lignes)
    : IRequest<Result<ResultatMasseDto>>;
