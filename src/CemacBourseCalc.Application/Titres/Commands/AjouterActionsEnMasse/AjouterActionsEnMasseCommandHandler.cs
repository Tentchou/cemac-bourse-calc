using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Titres.Commands.AjouterAction;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterActionsEnMasse;

public sealed class AjouterActionsEnMasseCommandHandler(ExecuteurEnMasse executeur)
    : IRequestHandler<AjouterActionsEnMasseCommand, Result<ResultatMasseDto>>
{
    public Task<Result<ResultatMasseDto>> Handle(AjouterActionsEnMasseCommand c, CancellationToken cancellationToken) =>
        executeur.ExecuterAsync<AjouterActionCommand, Guid>(c.Lignes, l => l.Isin, "Même ISIN", cancellationToken);
}
