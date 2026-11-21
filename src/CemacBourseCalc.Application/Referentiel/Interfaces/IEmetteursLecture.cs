using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Referentiel.Dtos;
using CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteursPage;

namespace CemacBourseCalc.Application.Referentiel.Interfaces;

/// <summary>Lecture des émetteurs (Dapper).</summary>
public interface IEmetteursLecture
{
    /// <summary>Tous les émetteurs, pour les choix des formulaires et l'import de fichier.</summary>
    Task<IReadOnlyList<EmetteurResumeDto>> ListerAsync(CancellationToken ct);

    /// <summary>Une page de la liste de l'administration.</summary>
    Task<PageDto<EmetteurAdminDto>> PageAsync(ListerEmetteursPageQuery q, CancellationToken ct);
}
