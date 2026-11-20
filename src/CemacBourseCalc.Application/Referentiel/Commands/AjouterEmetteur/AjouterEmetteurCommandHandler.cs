using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Referentiel.Interfaces;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Referentiel;
using MediatR;

namespace CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteur;

public sealed class AjouterEmetteurCommandHandler(
    IEmetteursEcriture ecriture,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail) : IRequestHandler<AjouterEmetteurCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AjouterEmetteurCommand c, CancellationToken cancellationToken)
    {
        if (courant.Id is not { } auteur) return Result<Guid>.Echec(Journal.MessageSessionRequise);

        Emetteur emetteur;
        try
        {
            emetteur = new Emetteur(c.Nom, c.Sigle, c.Type, c.PaysCode, c.SiteWeb);
        }
        catch (DomainException e)
        {
            return Result<Guid>.Echec(e.Message);
        }

        if (await ecriture.NomExisteAsync(emetteur.Nom, cancellationToken))
            return Result<Guid>.Echec($"L'émetteur {emetteur.Nom} existe déjà.");

        ecriture.Ajouter(emetteur);
        journal.Noter(auteur, Journal.Creation, Journal.EntiteEmetteur, emetteur.Id.ToString(), apres: c);
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<Guid>.Succes(emetteur.Id, $"L'émetteur {emetteur.Nom} est enregistré.");
    }
}
