using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using MediatR;
using Action = CemacBourseCalc.Domain.Titres.Action;

namespace CemacBourseCalc.Application.Titres.Commands.ModifierAction;

/// <summary>Applique la correction par le Domain et note au journal l'état avant et après.</summary>
public sealed class ModifierActionCommandHandler(
    ITitresEcriture ecriture,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail) : IRequestHandler<ModifierActionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ModifierActionCommand c, CancellationToken cancellationToken)
    {
        if (courant.Id is not { } auteur) return Result<Guid>.Echec(Journal.MessageSessionRequise);

        if (await ecriture.ActionAsync(c.Id, cancellationToken) is not { } action)
            return Result<Guid>.Echec("Cette action n'existe plus : rechargez la liste.");

        if (!await ecriture.EmetteurExisteAsync(c.EmetteurId, cancellationToken))
            return Result<Guid>.Echec("L'émetteur choisi n'existe pas : rechargez la page et choisissez-le dans la liste.");

        var avant = Etat(action);
        try
        {
            action.Modifier(c.Nom, c.EmetteurId, c.Nominal, c.DateAdmission, c.Statut, c.NombreTitres);
        }
        catch (DomainException e)
        {
            return Result<Guid>.Echec(e.Message);
        }

        var apres = Etat(action);
        if (avant == apres) return Result<Guid>.Succes(action.Id, "Aucun changement à enregistrer.");

        journal.Noter(auteur, Journal.Modification, Journal.EntiteTitre, action.Id.ToString(), avant, apres);
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<Guid>.Succes(action.Id, $"L'action {action.Nom} est modifiée.");
    }

    private static EtatAction Etat(Action a) => new(a.Nom, a.EmetteurId, a.Nominal, a.DateAdmission, a.Statut, a.NombreTitres);

    private sealed record EtatAction(string Nom, Guid EmetteurId, decimal Nominal, DateOnly? DateAdmission, StatutTitre Statut, long NombreTitres);
}
