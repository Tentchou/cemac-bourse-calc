using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerActionsAdmin;

/// <summary>Une page des actions, par nom. Recherche : nom, ISIN, émetteur ou sigle. Pays facultatif.</summary>
public sealed record ListerActionsAdminQuery(int Page, int Taille, string? Recherche, string? Pays)
    : IRequest<PageDto<ActionAdminDto>>, IRequetePaginee;
