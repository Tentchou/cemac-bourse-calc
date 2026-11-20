using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterCours;

public sealed class AjouterCoursCommandHandler(
    ITitresEcriture ecriture,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail) : IRequestHandler<AjouterCoursCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AjouterCoursCommand c, CancellationToken cancellationToken)
    {
        if (courant.Id is not { } auteur) return Result<Guid>.Echec(Journal.MessageSessionRequise);

        var type = await ecriture.TypeDuTitreAsync(c.TitreId, cancellationToken);
        if (type is null)
            return Result<Guid>.Echec("Le titre choisi n'existe pas : rechargez la page et choisissez-le dans la liste.");

        // Une obligation cote en % du nominal : un cours de 9 850 est presque toujours un montant en FCFA saisi par erreur.
        if (type == TypeTitre.Obligation
            && (c.CoursReference > PrixPleinCoupon.CoursMaximum || c.CoursCloture > PrixPleinCoupon.CoursMaximum))
            return Result<Guid>.Echec("Le cours d'une obligation s'exprime en pourcentage du nominal (ex. 98,50), pas en FCFA.");

        if (await ecriture.CoursExisteAsync(c.TitreId, c.DateSeance, cancellationToken))
            return Result<Guid>.Echec($"Le cours de la séance du {c.DateSeance:dd/MM/yyyy} est déjà saisi pour ce titre.");

        Cours cours;
        try
        {
            cours = new Cours(c.TitreId, c.DateSeance, c.CoursReference, c.CoursCloture, c.Volume, c.CouponCouruPublie);
        }
        catch (DomainException e)
        {
            return Result<Guid>.Echec(e.Message);
        }

        ecriture.AjouterCours(cours);
        journal.Noter(auteur, Journal.Creation, Journal.EntiteCours, cours.Id.ToString(), apres: c);
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<Guid>.Succes(cours.Id, $"Le cours du {c.DateSeance:dd/MM/yyyy} est enregistré.");
    }
}
