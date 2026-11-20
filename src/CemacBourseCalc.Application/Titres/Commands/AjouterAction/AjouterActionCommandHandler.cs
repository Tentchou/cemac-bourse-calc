using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using MediatR;
using Action = CemacBourseCalc.Domain.Titres.Action;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterAction;

public sealed class AjouterActionCommandHandler(
    ITitresEcriture ecriture,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail) : IRequestHandler<AjouterActionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AjouterActionCommand c, CancellationToken cancellationToken)
    {
        if (courant.Id is not { } auteur) return Result<Guid>.Echec(Journal.MessageSessionRequise);

        Action action;
        try
        {
            action = new Action(Isin.Creer(c.Isin), c.Nom, c.EmetteurId, c.Nominal, c.DateAdmission, c.NombreTitres);
        }
        catch (DomainException e)
        {
            return Result<Guid>.Echec(e.Message);
        }

        if (!await ecriture.EmetteurExisteAsync(action.EmetteurId, cancellationToken))
            return Result<Guid>.Echec("L'émetteur choisi n'existe pas : rechargez la page et choisissez-le dans la liste.");

        if (await ecriture.IsinExisteAsync(action.Isin.Valeur, cancellationToken))
            return Result<Guid>.Echec($"Le titre {action.Isin} existe déjà.");

        ecriture.Ajouter(action);
        journal.Noter(auteur, Journal.Creation, Journal.EntiteTitre, action.Id.ToString(), apres: c);
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<Guid>.Succes(action.Id, $"L'action {action.Nom} est enregistrée.");
    }
}
