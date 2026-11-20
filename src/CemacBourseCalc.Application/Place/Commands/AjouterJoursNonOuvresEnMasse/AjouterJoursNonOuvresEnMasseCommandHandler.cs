using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Place.Commands.AjouterJourNonOuvre;
using MediatR;

namespace CemacBourseCalc.Application.Place.Commands.AjouterJoursNonOuvresEnMasse;

public sealed class AjouterJoursNonOuvresEnMasseCommandHandler(ExecuteurEnMasse executeur)
    : IRequestHandler<AjouterJoursNonOuvresEnMasseCommand, Result<ResultatMasseDto>>
{
    public Task<Result<ResultatMasseDto>> Handle(AjouterJoursNonOuvresEnMasseCommand c, CancellationToken cancellationToken) =>
        executeur.ExecuterAsync<AjouterJourNonOuvreCommand, DateOnly>(
            c.Lignes, l => l.Date.ToString("yyyy-MM-dd"), "Même date", cancellationToken);
}
