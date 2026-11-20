using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Titres.Commands.AjouterDividende;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterDividendesEnMasse;

/// <summary>Pas de contrôle de doublon : un acompte et un solde du même exercice sont deux lignes légitimes.</summary>
public sealed class AjouterDividendesEnMasseCommandHandler(ExecuteurEnMasse executeur)
    : IRequestHandler<AjouterDividendesEnMasseCommand, Result<ResultatMasseDto>>
{
    public Task<Result<ResultatMasseDto>> Handle(AjouterDividendesEnMasseCommand c, CancellationToken cancellationToken) =>
        executeur.ExecuterAsync<AjouterDividendeCommand, Guid>(c.Lignes, null, string.Empty, cancellationToken);
}
