using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Commands.FermerSession;

public sealed class FermerSessionCommandHandler(
    IAdministrateursEcriture administrateurs,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail,
    TimeProvider horloge) : IRequestHandler<FermerSessionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(FermerSessionCommand c, CancellationToken cancellationToken)
    {
        // Déjà fermée, expirée ou inconnue : rien à faire, la déconnexion réussit quand même.
        if (!JetonsSession.FormatValide(c.Jeton)) return Result<bool>.Succes(false);

        var session = await administrateurs.SessionParEmpreinteAsync(JetonsSession.Empreinte(c.Jeton), cancellationToken);
        if (session is null || session.RevoqueLe is not null) return Result<bool>.Succes(false);

        session.Revoquer(horloge.GetUtcNow());
        journal.Noter(session.AdministrateurId, Journal.Deconnexion, Journal.EntiteAdministrateur, session.AdministrateurId.ToString());
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<bool>.Succes(true);
    }
}
