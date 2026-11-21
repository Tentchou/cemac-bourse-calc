using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerActionsAdmin;

public sealed class ListerActionsAdminQueryHandler(ITitresAdminLecture lecture)
    : IRequestHandler<ListerActionsAdminQuery, PageDto<ActionAdminDto>>
{
    public Task<PageDto<ActionAdminDto>> Handle(ListerActionsAdminQuery q, CancellationToken cancellationToken) =>
        lecture.ActionsAsync(q, cancellationToken);
}
