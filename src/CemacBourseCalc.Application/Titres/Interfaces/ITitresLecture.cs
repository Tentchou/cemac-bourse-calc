using CemacBourseCalc.Application.Titres.Dtos;

namespace CemacBourseCalc.Application.Titres.Interfaces;

/// <summary>Lectures des titres (Dapper, SQL écrit à la main).</summary>
public interface ITitresLecture
{
    Task<IReadOnlyList<TitreResumeDto>> ListerAsync(CancellationToken ct);
}
