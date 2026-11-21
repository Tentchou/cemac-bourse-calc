using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ObtenirHistoriqueCours;

public sealed class ObtenirHistoriqueCoursQueryHandler(IHistoriqueCoursLecture lecture)
    : IRequestHandler<ObtenirHistoriqueCoursQuery, IReadOnlyList<PointCoursDto>?>
{
    public Task<IReadOnlyList<PointCoursDto>?> Handle(ObtenirHistoriqueCoursQuery request, CancellationToken cancellationToken) =>
        lecture.HistoriqueAsync(request.TitreId, ObtenirHistoriqueCoursQuery.SeancesMaximum, cancellationToken);
}
