using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterObligation;

public sealed class AjouterObligationCommandHandler(
    ITitresEcriture ecriture,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail) : IRequestHandler<AjouterObligationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AjouterObligationCommand c, CancellationToken cancellationToken)
    {
        if (courant.Id is not { } auteur) return Result<Guid>.Echec(Journal.MessageSessionRequise);

        Obligation obligation;
        try
        {
            obligation = new Obligation(
                Isin.Creer(c.Isin), c.Nom, c.EmetteurId, c.Nominal, c.DateAdmission,
                c.TauxFacial, c.Periodicite, c.DateJouissance, c.DatePremierCoupon, c.DateEcheance,
                c.ModeRemboursement, c.DiffereEcheances, c.CodeConvention, c.RegimeFiscal);
        }
        catch (DomainException e)
        {
            return Result<Guid>.Echec(e.Message);
        }

        if (!await ecriture.EmetteurExisteAsync(obligation.EmetteurId, cancellationToken))
            return Result<Guid>.Echec("L'émetteur choisi n'existe pas : rechargez la page et choisissez-le dans la liste.");

        if (await ecriture.IsinExisteAsync(obligation.Isin.Valeur, cancellationToken))
            return Result<Guid>.Echec($"Le titre {obligation.Isin} existe déjà.");

        ecriture.Ajouter(obligation);
        journal.Noter(auteur, Journal.Creation, Journal.EntiteTitre, obligation.Id.ToString(), apres: c);
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<Guid>.Succes(obligation.Id, $"L'obligation {obligation.Nom} est enregistrée.");
    }
}
