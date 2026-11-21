using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Place.Dtos;
using CemacBourseCalc.Application.Place.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Place.Queries.ListerJoursNonOuvres;

public sealed class ListerJoursNonOuvresQueryHandler(IJoursNonOuvresLecture lecture)
    : IRequestHandler<ListerJoursNonOuvresQuery, PageDto<JourNonOuvreDto>>
{
    public Task<PageDto<JourNonOuvreDto>> Handle(ListerJoursNonOuvresQuery q, CancellationToken cancellationToken) =>
        lecture.PageAsync(q, cancellationToken);
}
