using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Calculs.Queries.CalculerRendementDividende;

public sealed class CalculerRendementDividendeQueryHandler(ILectureCalculs lecture)
    : IRequestHandler<CalculerRendementDividendeQuery, Result<RendementDividendeDto>>
{
    public async Task<Result<RendementDividendeDto>> Handle(CalculerRendementDividendeQuery q, CancellationToken cancellationToken)
    {
        var action = await lecture.DernierDividendeAsync(q.TitreId, cancellationToken);
        if (action is null)
            return Result<RendementDividendeDto>.Echec("Ce titre n'existe pas ou n'est pas une action.");
        if (action is not { Exercice: { } exercice, Brut: { } brut })
            return Result<RendementDividendeDto>.Echec($"Aucun dividende n'est encore saisi pour {action.Nom}.");

        try
        {
            var r = RendementDividende.Calculer(q.Cours, exercice, brut, action.Net);
            return Result<RendementDividendeDto>.Succes(new RendementDividendeDto(
                action.Nom, r.Cours, r.Exercice, action.NombreVersements,
                r.DividendeBrut, r.DividendeNet, r.RendementBrut, r.RendementNet));
        }
        catch (DomainException e)
        {
            return Result<RendementDividendeDto>.Echec(e.Message);
        }
    }
}
