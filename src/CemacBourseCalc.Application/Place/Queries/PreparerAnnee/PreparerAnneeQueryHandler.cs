using CemacBourseCalc.Application.Place.Dtos;
using CemacBourseCalc.Application.Place.Interfaces;
using CemacBourseCalc.Domain.Place;
using MediatR;

namespace CemacBourseCalc.Application.Place.Queries.PreparerAnnee;

/// <summary>Croise le calcul du Domain avec le calendrier déjà saisi pour l'année (une seule lecture).</summary>
public sealed class PreparerAnneeQueryHandler(IJoursNonOuvresLecture lecture) : IRequestHandler<PreparerAnneeQuery, AnneePrepareeDto>
{
    public async Task<AnneePrepareeDto> Handle(PreparerAnneeQuery q, CancellationToken cancellationToken)
    {
        var enregistres = (await lecture.DeLAnneeAsync(q.Annee, cancellationToken)).ToDictionary(j => j.Date, j => j.Libelle);
        var calcules = JoursFeriesCameroun.Calculer(q.Annee);

        var proposes = calcules
            .Select(j => new JourProposeDto(j.Date, j.Libelle, enregistres.ContainsKey(j.Date), enregistres.GetValueOrDefault(j.Date)))
            .ToList();
        var dates = calcules.Select(j => j.Date).ToHashSet();
        var autres = enregistres
            .Where(e => !dates.Contains(e.Key))
            .OrderBy(e => e.Key)
            .Select(e => new JourNonOuvreDto(e.Key, e.Value))
            .ToList();

        return new AnneePrepareeDto(q.Annee, proposes, autres);
    }
}
