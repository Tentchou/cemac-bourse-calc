using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Titres.Commands.AjouterObligation;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterObligationsEnMasse;

public sealed class AjouterObligationsEnMasseCommandHandler(ExecuteurEnMasse executeur)
    : IRequestHandler<AjouterObligationsEnMasseCommand, Result<ResultatMasseDto>>
{
    public Task<Result<ResultatMasseDto>> Handle(AjouterObligationsEnMasseCommand c, CancellationToken cancellationToken) =>
        executeur.ExecuterAsync<AjouterObligationCommand, Guid>(c.Lignes, l => l.Isin, "Même ISIN", cancellationToken);
}
