using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Titres.Commands.AjouterDividende;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterDividendesEnMasse;

/// <summary>Plusieurs versements d'un coup : chaque ligne est une saisie manuelle (<see cref="AjouterDividendeCommand"/>).</summary>
public sealed record AjouterDividendesEnMasseCommand(IReadOnlyList<AjouterDividendeCommand> Lignes)
    : IRequest<Result<ResultatMasseDto>>;
