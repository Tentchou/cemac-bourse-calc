using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Dtos;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Domain.Administration;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Commands.OuvrirSession;

/// <summary>
/// Un seul message d'échec, que l'e-mail soit inconnu, le compte désactivé ou le mot de passe faux :
/// la réponse ne dit pas quels comptes existent. Le nombre d'essais est limité par adresse IP (API).
/// </summary>
public sealed class OuvrirSessionCommandHandler(
    IAdministrateursEcriture administrateurs,
    IHacheurMotDePasse hacheur,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail,
    TimeProvider horloge) : IRequestHandler<OuvrirSessionCommand, Result<SessionOuverteDto>>
{
    public const string MessageEchec = "E-mail ou mot de passe incorrect.";

    public async Task<Result<SessionOuverteDto>> Handle(OuvrirSessionCommand c, CancellationToken cancellationToken)
    {
        var admin = await administrateurs.ParEmailAsync(c.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (admin is null || !admin.Actif)
        {
            hacheur.VerifierFactice(c.MotDePasse);
            return Result<SessionOuverteDto>.Echec(MessageEchec);
        }

        var verification = hacheur.Verifier(admin.MotDePasseHash, c.MotDePasse);
        if (verification == VerificationMotDePasse.Incorrect)
            return Result<SessionOuverteDto>.Echec(MessageEchec);
        if (verification == VerificationMotDePasse.CorrectARehacher)
            admin.ChangerMotDePasse(hacheur.Hacher(c.MotDePasse));

        var maintenant = horloge.GetUtcNow();
        var (jeton, empreinte) = JetonsSession.Generer();
        var session = new JetonSession(admin.Id, empreinte, maintenant, maintenant + JetonsSession.Duree);

        administrateurs.AjouterSession(session);
        admin.NoterConnexion(maintenant);
        journal.Noter(admin.Id, Journal.Connexion, Journal.EntiteAdministrateur, admin.Id.ToString());
        await uniteDeTravail.EnregistrerAsync(cancellationToken);

        return Result<SessionOuverteDto>.Succes(new SessionOuverteDto(jeton, session.ExpireLe, admin.Nom, admin.Role));
    }
}
