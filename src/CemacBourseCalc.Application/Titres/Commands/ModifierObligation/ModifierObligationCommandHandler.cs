using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.ModifierObligation;

/// <summary>
/// Applique la correction par le Domain (mêmes règles qu'à la création) et note au journal
/// l'état avant et après : on sait toujours qui a changé quoi.
/// </summary>
public sealed class ModifierObligationCommandHandler(
    ITitresEcriture ecriture,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail) : IRequestHandler<ModifierObligationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ModifierObligationCommand c, CancellationToken cancellationToken)
    {
        if (courant.Id is not { } auteur) return Result<Guid>.Echec(Journal.MessageSessionRequise);

        if (await ecriture.ObligationAsync(c.Id, cancellationToken) is not { } obligation)
            return Result<Guid>.Echec("Cette obligation n'existe plus : rechargez la liste.");

        if (!await ecriture.EmetteurExisteAsync(c.EmetteurId, cancellationToken))
            return Result<Guid>.Echec("L'émetteur choisi n'existe pas : rechargez la page et choisissez-le dans la liste.");

        var avant = Etat(obligation);
        try
        {
            obligation.Modifier(c.Nom, c.EmetteurId, c.Nominal, c.DateAdmission, c.Statut,
                c.TauxFacial, c.Periodicite, c.DateJouissance, c.DatePremierCoupon, c.DateEcheance,
                c.ModeRemboursement, c.DiffereEcheances, c.CodeConvention, c.RegimeFiscal);
        }
        catch (DomainException e)
        {
            return Result<Guid>.Echec(e.Message);
        }

        var apres = Etat(obligation);
        if (avant == apres) return Result<Guid>.Succes(obligation.Id, "Aucun changement à enregistrer.");

        journal.Noter(auteur, Journal.Modification, Journal.EntiteTitre, obligation.Id.ToString(), avant, apres);
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<Guid>.Succes(obligation.Id, $"L'obligation {obligation.Nom} est modifiée.");
    }

    /// <summary>Photographie des champs modifiables : pour le journal, et pour voir que rien n'a changé.</summary>
    private static EtatObligation Etat(Obligation o) => new(
        o.Nom, o.EmetteurId, o.Nominal, o.DateAdmission, o.Statut, o.TauxFacial, o.Periodicite, o.DateJouissance,
        o.DatePremierCoupon, o.DateEcheance, o.ModeRemboursement, o.DiffereEcheances, o.CodeConvention, o.RegimeFiscal);

    private sealed record EtatObligation(
        string Nom, Guid EmetteurId, decimal Nominal, DateOnly? DateAdmission, StatutTitre Statut, decimal TauxFacial,
        Periodicite Periodicite, DateOnly DateJouissance, DateOnly? DatePremierCoupon, DateOnly DateEcheance,
        ModeRemboursement ModeRemboursement, int DiffereEcheances, string CodeConvention, RegimeFiscal RegimeFiscal);
}
