using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Place.Dtos;
using CemacBourseCalc.Application.Place.Queries.ListerJoursNonOuvres;

namespace CemacBourseCalc.Application.Place.Interfaces;

/// <summary>Lecture du calendrier de la place pour l'administration (Dapper).</summary>
public interface IJoursNonOuvresLecture
{
    Task<PageDto<JourNonOuvreDto>> PageAsync(ListerJoursNonOuvresQuery q, CancellationToken ct);
}
