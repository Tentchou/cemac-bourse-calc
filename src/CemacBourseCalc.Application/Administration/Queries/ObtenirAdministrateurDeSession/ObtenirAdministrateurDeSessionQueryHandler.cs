using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Dtos;
using CemacBourseCalc.Application.Administration.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Queries.ObtenirAdministrateurDeSession;

public sealed class ObtenirAdministrateurDeSessionQueryHandler(ISessionsLecture sessions, TimeProvider horloge)
    : IRequestHandler<ObtenirAdministrateurDeSessionQuery, AdministrateurConnecteDto?>
{
    public async Task<AdministrateurConnecteDto?> Handle(ObtenirAdministrateurDeSessionQuery q, CancellationToken cancellationToken) =>
        JetonsSession.FormatValide(q.Jeton)
            ? await sessions.AdministrateurDeSessionAsync(JetonsSession.Empreinte(q.Jeton), horloge.GetUtcNow(), cancellationToken)
            : null;
}
