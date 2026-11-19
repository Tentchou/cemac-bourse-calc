using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterObligation;

public sealed class AjouterObligationCommandHandler(ITitresEcriture ecriture)
    : IRequestHandler<AjouterObligationCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AjouterObligationCommand c, CancellationToken cancellationToken)
    {
        Obligation obligation;
        try
        {
            obligation = new Obligation(
                Isin.Creer(c.Isin), c.Nom, c.Emetteur, c.Nominal, c.TauxFacial, c.Periodicite,
                c.DateJouissance, c.DateEcheance, c.ModeRemboursement, c.DiffereEcheances, c.CodeConvention);
        }
        catch (DomainException e)
        {
            return Result<int>.Echec(e.Message);
        }

        if (await ecriture.IsinExisteAsync(obligation.Isin.Valeur, cancellationToken))
            return Result<int>.Echec($"Le titre {obligation.Isin} existe déjà.");

        var id = await ecriture.AjouterAsync(obligation, cancellationToken);
        return Result<int>.Succes(id, $"L'obligation {obligation.Nom} est enregistrée.");
    }
}
