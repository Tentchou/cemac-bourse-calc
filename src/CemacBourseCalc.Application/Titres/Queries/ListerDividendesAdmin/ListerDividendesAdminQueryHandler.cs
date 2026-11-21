using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerDividendesAdmin;

public sealed class ListerDividendesAdminQueryHandler(IMarcheAdminLecture lecture)
    : IRequestHandler<ListerDividendesAdminQuery, PageDto<DividendeAdminDto>>
{
    public Task<PageDto<DividendeAdminDto>> Handle(ListerDividendesAdminQuery q, CancellationToken cancellationToken) =>
        lecture.DividendesAsync(q, cancellationToken);
}
