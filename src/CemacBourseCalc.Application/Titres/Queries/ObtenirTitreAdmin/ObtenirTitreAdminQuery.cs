using CemacBourseCalc.Application.Titres.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ObtenirTitreAdmin;

/// <summary>Un titre à modifier, avec la taille de son historique. Null : titre inconnu (404).</summary>
public sealed record ObtenirTitreAdminQuery(Guid Id) : IRequest<TitreAdminDetailDto?>;
