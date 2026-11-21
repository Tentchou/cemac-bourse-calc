using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerDividendesAdmin;

/// <summary>Une page des dividendes, du plus récent exercice au plus ancien. Recherche : nom ou ISIN.</summary>
public sealed record ListerDividendesAdminQuery(int Page, int Taille, string? Recherche)
    : IRequest<PageDto<DividendeAdminDto>>, IRequetePaginee;
