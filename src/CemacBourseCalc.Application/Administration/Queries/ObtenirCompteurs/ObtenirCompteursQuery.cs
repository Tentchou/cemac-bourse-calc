using CemacBourseCalc.Application.Administration.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Queries.ObtenirCompteurs;

/// <summary>Le nombre de lignes de chaque rubrique de l'administration.</summary>
public sealed record ObtenirCompteursQuery : IRequest<CompteursAdminDto>;
