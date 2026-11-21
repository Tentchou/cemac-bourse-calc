using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Queries.ListerCoursAdmin;
using CemacBourseCalc.Application.Titres.Queries.ListerDividendesAdmin;

namespace CemacBourseCalc.Application.Titres.Interfaces;

/// <summary>Les listes paginées des données de marché pour l'administration (Dapper).</summary>
public interface IMarcheAdminLecture
{
    /// <summary>Les cours d'une séance ; sans séance précisée, ceux de la dernière séance saisie.</summary>
    Task<PageDto<CoursAdminLigne>> CoursAsync(ListerCoursAdminQuery q, CancellationToken ct);

    /// <summary>Les dates des séances saisies, de la plus récente à la plus ancienne.</summary>
    Task<IReadOnlyList<DateOnly>> SeancesAsync(int nombre, CancellationToken ct);

    Task<PageDto<DividendeAdminDto>> DividendesAsync(ListerDividendesAdminQuery q, CancellationToken ct);
}
