using CemacBourseCalc.Application.Administration.Dtos;

namespace CemacBourseCalc.Application.Administration.Interfaces;

/// <summary>Les compteurs du menu de l'administration (une seule requête).</summary>
public interface ICompteursLecture
{
    Task<CompteursAdminDto> CompteursAsync(CancellationToken ct);
}
