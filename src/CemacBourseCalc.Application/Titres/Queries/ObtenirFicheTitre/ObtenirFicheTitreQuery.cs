using CemacBourseCalc.Application.Titres.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ObtenirFicheTitre;

/// <summary>La fiche publique d'un titre. Null : titre inconnu (404).</summary>
public sealed record ObtenirFicheTitreQuery(Guid Id) : IRequest<FicheTitreDto?>;
