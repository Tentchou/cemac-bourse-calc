using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Place.Interfaces;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Place;
using MediatR;

namespace CemacBourseCalc.Application.Place.Commands.AjouterJourNonOuvre;

public sealed class AjouterJourNonOuvreCommandHandler(
    IJoursNonOuvresEcriture ecriture,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail) : IRequestHandler<AjouterJourNonOuvreCommand, Result<DateOnly>>
{
    public async Task<Result<DateOnly>> Handle(AjouterJourNonOuvreCommand c, CancellationToken cancellationToken)
    {
        if (courant.Id is not { } auteur) return Result<DateOnly>.Echec(Journal.MessageSessionRequise);

        if (c.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return Result<DateOnly>.Echec($"Le {c.Date:dd/MM/yyyy} tombe un week-end : il est déjà non ouvré.");

        if (await ecriture.ExisteAsync(c.Date, cancellationToken))
            return Result<DateOnly>.Echec($"Le {c.Date:dd/MM/yyyy} est déjà dans le calendrier.");

        JourNonOuvre jour;
        try
        {
            jour = new JourNonOuvre(c.Date, c.Libelle);
        }
        catch (DomainException e)
        {
            return Result<DateOnly>.Echec(e.Message);
        }

        ecriture.Ajouter(jour);
        journal.Noter(auteur, Journal.Creation, Journal.EntiteJourNonOuvre, jour.Date.ToString("yyyy-MM-dd"), apres: c);
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<DateOnly>.Succes(jour.Date, $"Le {jour.Date:dd/MM/yyyy} ({jour.Libelle}) est ajouté au calendrier.");
    }
}
