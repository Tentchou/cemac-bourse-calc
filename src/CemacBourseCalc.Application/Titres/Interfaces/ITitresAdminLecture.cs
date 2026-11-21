using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Queries.ListerActionsAdmin;
using CemacBourseCalc.Application.Titres.Queries.ListerObligationsAdmin;

namespace CemacBourseCalc.Application.Titres.Interfaces;

/// <summary>Les listes paginées des titres pour l'administration (Dapper, une page à la fois).</summary>
public interface ITitresAdminLecture
{
    Task<PageDto<ObligationAdminLigne>> ObligationsAsync(ListerObligationsAdminQuery q, CancellationToken ct);
    Task<PageDto<ActionAdminDto>> ActionsAsync(ListerActionsAdminQuery q, CancellationToken ct);
}
