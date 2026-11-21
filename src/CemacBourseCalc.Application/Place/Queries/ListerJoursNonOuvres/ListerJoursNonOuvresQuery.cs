using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Place.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Place.Queries.ListerJoursNonOuvres;

/// <summary>Une page des jours non ouvrés, du plus récent au plus ancien. Année facultative ; recherche dans le libellé.</summary>
public sealed record ListerJoursNonOuvresQuery(int Page, int Taille, string? Recherche, int? Annee)
    : IRequest<PageDto<JourNonOuvreDto>>, IRequetePaginee;
