using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Titres.Commands.AjouterCours;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterCoursEnMasse;

public sealed class AjouterCoursEnMasseCommandHandler(ExecuteurEnMasse executeur)
    : IRequestHandler<AjouterCoursEnMasseCommand, Result<ResultatMasseDto>>
{
    public Task<Result<ResultatMasseDto>> Handle(AjouterCoursEnMasseCommand c, CancellationToken cancellationToken) =>
        executeur.ExecuterAsync<AjouterCoursCommand, Guid>(
            c.Lignes, l => $"{l.TitreId}|{l.DateSeance:yyyy-MM-dd}", "Même titre et même séance", cancellationToken);
}
