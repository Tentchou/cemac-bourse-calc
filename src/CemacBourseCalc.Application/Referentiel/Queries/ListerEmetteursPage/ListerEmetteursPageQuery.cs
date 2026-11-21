using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Referentiel.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteursPage;

/// <summary>Une page des émetteurs, par nom. Recherche : nom ou sigle. Pays et type facultatifs.</summary>
public sealed record ListerEmetteursPageQuery(int Page, int Taille, string? Recherche, string? Pays, string? Type)
    : IRequest<PageDto<EmetteurAdminDto>>, IRequetePaginee;
