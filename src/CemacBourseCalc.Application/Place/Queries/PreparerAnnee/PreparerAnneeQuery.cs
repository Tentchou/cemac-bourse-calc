using CemacBourseCalc.Application.Place.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Place.Queries.PreparerAnnee;

/// <summary>
/// L'aperçu du calendrier d'une année, sans rien enregistrer. L'administrateur valide ensuite, et les jours
/// à ajouter passent par la commande de saisie en masse : les mêmes règles qu'une saisie à la main.
/// </summary>
public sealed record PreparerAnneeQuery(int Annee) : IRequest<AnneePrepareeDto>;
