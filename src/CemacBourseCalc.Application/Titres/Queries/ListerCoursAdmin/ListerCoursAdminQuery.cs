using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerCoursAdmin;

/// <summary>Une page des cours d'une séance (la dernière saisie par défaut), par nom de titre. Recherche : nom ou ISIN.</summary>
public sealed record ListerCoursAdminQuery(int Page, int Taille, string? Recherche, DateOnly? DateSeance)
    : IRequest<PageDto<CoursAdminDto>>, IRequetePaginee;
