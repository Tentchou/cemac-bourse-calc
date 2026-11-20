using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteur;
using MediatR;

namespace CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteursEnMasse;

/// <summary>Plusieurs émetteurs d'un coup : chaque ligne est une saisie manuelle (<see cref="AjouterEmetteurCommand"/>).</summary>
public sealed record AjouterEmetteursEnMasseCommand(IReadOnlyList<AjouterEmetteurCommand> Lignes)
    : IRequest<Result<ResultatMasseDto>>;
