using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Domain.Administration;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Commands.CreerAdministrateur;

public sealed class CreerAdministrateurCommandHandler(
    IAdministrateursEcriture administrateurs,
    IHacheurMotDePasse hacheur,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail,
    TimeProvider horloge) : IRequestHandler<CreerAdministrateurCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreerAdministrateurCommand c, CancellationToken cancellationToken)
    {
        var email = c.Email.Trim().ToLowerInvariant();
        if (await administrateurs.EmailExisteAsync(email, cancellationToken))
            return Result<Guid>.Echec($"Un compte existe déjà avec l'adresse {email}.");

        Administrateur admin;
        try
        {
            admin = new Administrateur(email, c.Nom, hacheur.Hacher(c.MotDePasse), c.Role, horloge.GetUtcNow());
        }
        catch (DomainException e)
        {
            return Result<Guid>.Echec(e.Message);
        }

        administrateurs.Ajouter(admin);
        // Hors session (premier compte, créé sur le serveur), l'auteur est le compte lui-même.
        journal.Noter(courant.Id ?? admin.Id, Journal.Creation, Journal.EntiteAdministrateur, admin.Id.ToString(),
            apres: new { admin.Email, admin.Nom, admin.Role });
        await uniteDeTravail.EnregistrerAsync(cancellationToken);

        return Result<Guid>.Succes(admin.Id, $"Le compte de {admin.Nom} ({admin.Role}) est créé.");
    }
}
