using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Referentiel.Dtos;
using CemacBourseCalc.Application.Referentiel.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteursPage;

public sealed class ListerEmetteursPageQueryHandler(IEmetteursLecture lecture)
    : IRequestHandler<ListerEmetteursPageQuery, PageDto<EmetteurAdminDto>>
{
    public Task<PageDto<EmetteurAdminDto>> Handle(ListerEmetteursPageQuery q, CancellationToken cancellationToken) =>
        lecture.PageAsync(q, cancellationToken);
}
