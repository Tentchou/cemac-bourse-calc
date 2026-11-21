using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerObligationsAdmin;

/// <summary>Une page des obligations, par nom. Recherche : nom, ISIN, émetteur ou sigle. Pays et remboursement facultatifs.</summary>
public sealed record ListerObligationsAdminQuery(int Page, int Taille, string? Recherche, string? Pays, string? Remboursement)
    : IRequest<PageDto<ObligationAdminDto>>, IRequetePaginee;
