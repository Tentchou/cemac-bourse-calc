using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.SupprimerTitre;

public sealed class SupprimerTitreCommandHandler(
    ITitresEcriture ecriture,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail) : IRequestHandler<SupprimerTitreCommand, Result<SuppressionTitreDto>>
{
    public async Task<Result<SuppressionTitreDto>> Handle(SupprimerTitreCommand c, CancellationToken cancellationToken)
    {
        if (courant.Id is not { } auteur) return Result<SuppressionTitreDto>.Echec(Journal.MessageSessionRequise);

        if (await ecriture.TitreAsync(c.Id, cancellationToken) is not { } titre)
            return Result<SuppressionTitreDto>.Echec("Ce titre n'existe plus : rechargez la liste.");

        var avant = new { Isin = titre.Isin.Valeur, titre.Nom, titre.TypeTitre, titre.Statut };

        // Un titre qui a des cours, des dividendes ou des documents n'est jamais effacé : il est radié.
        if (await ecriture.AHistoriqueAsync(titre.Id, cancellationToken))
        {
            if (titre.Statut == StatutTitre.Radie)
                return Result<SuppressionTitreDto>.Echec($"Le titre {titre.Nom} est déjà radié ; son historique est gardé.");

            titre.Radier();
            journal.Noter(auteur, Journal.Radiation, Journal.EntiteTitre, titre.Id.ToString(), avant, new { titre.Statut });
            await uniteDeTravail.EnregistrerAsync(cancellationToken);
            return Result<SuppressionTitreDto>.Succes(new SuppressionTitreDto(titre.Id, false),
                $"Le titre {titre.Nom} est radié : il n'apparaît plus sur le site, son historique est gardé.");
        }

        ecriture.Supprimer(titre);
        journal.Noter(auteur, Journal.Suppression, Journal.EntiteTitre, titre.Id.ToString(), avant);
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<SuppressionTitreDto>.Succes(new SuppressionTitreDto(titre.Id, true), $"Le titre {titre.Nom} est effacé.");
    }
}
