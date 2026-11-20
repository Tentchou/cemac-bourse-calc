using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Titres.Commands.AjouterCours;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterCoursEnMasse;

/// <summary>Un bulletin de cote entier : chaque ligne est une saisie manuelle (<see cref="AjouterCoursCommand"/>).</summary>
public sealed record AjouterCoursEnMasseCommand(IReadOnlyList<AjouterCoursCommand> Lignes)
    : IRequest<Result<ResultatMasseDto>>;
