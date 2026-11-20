using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteur;
using MediatR;

namespace CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteursEnMasse;

public sealed class AjouterEmetteursEnMasseCommandHandler(ExecuteurEnMasse executeur)
    : IRequestHandler<AjouterEmetteursEnMasseCommand, Result<ResultatMasseDto>>
{
    public Task<Result<ResultatMasseDto>> Handle(AjouterEmetteursEnMasseCommand c, CancellationToken cancellationToken) =>
        executeur.ExecuterAsync<AjouterEmetteurCommand, Guid>(c.Lignes, l => l.Nom, "Même nom d'émetteur", cancellationToken);
}
