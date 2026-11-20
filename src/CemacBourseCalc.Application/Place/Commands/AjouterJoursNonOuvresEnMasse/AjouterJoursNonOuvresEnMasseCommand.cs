using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Place.Commands.AjouterJourNonOuvre;
using MediatR;

namespace CemacBourseCalc.Application.Place.Commands.AjouterJoursNonOuvresEnMasse;

/// <summary>Le calendrier d'une année d'un coup : chaque ligne est une saisie manuelle (<see cref="AjouterJourNonOuvreCommand"/>).</summary>
public sealed record AjouterJoursNonOuvresEnMasseCommand(IReadOnlyList<AjouterJourNonOuvreCommand> Lignes)
    : IRequest<Result<ResultatMasseDto>>;
