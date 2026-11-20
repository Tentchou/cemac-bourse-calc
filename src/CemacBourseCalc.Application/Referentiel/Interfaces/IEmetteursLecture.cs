using CemacBourseCalc.Application.Referentiel.Dtos;

namespace CemacBourseCalc.Application.Referentiel.Interfaces;

/// <summary>Lecture des émetteurs (Dapper).</summary>
public interface IEmetteursLecture
{
    Task<IReadOnlyList<EmetteurResumeDto>> ListerAsync(CancellationToken ct);
}
