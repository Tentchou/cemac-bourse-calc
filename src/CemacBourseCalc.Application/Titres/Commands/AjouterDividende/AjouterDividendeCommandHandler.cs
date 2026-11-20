using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterDividende;

public sealed class AjouterDividendeCommandHandler(
    ITitresEcriture ecriture,
    IAdministrateurCourant courant,
    IJournalAdmin journal,
    IUniteDeTravail uniteDeTravail) : IRequestHandler<AjouterDividendeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AjouterDividendeCommand c, CancellationToken cancellationToken)
    {
        if (courant.Id is not { } auteur) return Result<Guid>.Echec(Journal.MessageSessionRequise);

        var type = await ecriture.TypeDuTitreAsync(c.TitreId, cancellationToken);
        if (type is null)
            return Result<Guid>.Echec("Le titre choisi n'existe pas : rechargez la page et choisissez-le dans la liste.");
        if (type != TypeTitre.Action)
            return Result<Guid>.Echec("Un dividende ne se saisit que pour une action.");

        Dividende dividende;
        try
        {
            dividende = new Dividende(c.TitreId, c.Exercice, c.MontantBrut, c.MontantNet, c.DateDetachement, c.DatePaiement);
        }
        catch (DomainException e)
        {
            return Result<Guid>.Echec(e.Message);
        }

        ecriture.AjouterDividende(dividende);
        journal.Noter(auteur, Journal.Creation, Journal.EntiteDividende, dividende.Id.ToString(), apres: c);
        await uniteDeTravail.EnregistrerAsync(cancellationToken);
        return Result<Guid>.Succes(dividende.Id, $"Le dividende de l'exercice {c.Exercice} est enregistré.");
    }
}
