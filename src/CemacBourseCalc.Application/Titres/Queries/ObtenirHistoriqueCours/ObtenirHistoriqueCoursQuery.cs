using CemacBourseCalc.Application.Titres.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ObtenirHistoriqueCours;

/// <summary>
/// L'historique public des cours d'un titre, en une seule réponse : le site choisit ensuite la période
/// (1 mois, 3 mois…) sans rappeler l'API. Null : titre inconnu (404).
/// </summary>
public sealed record ObtenirHistoriqueCoursQuery(Guid TitreId) : IRequest<IReadOnlyList<PointCoursDto>?>
{
    /// <summary>Environ cinq ans de séances quotidiennes : la réponse reste petite (quelques dizaines de Ko).</summary>
    public const int SeancesMaximum = 1300;
}
