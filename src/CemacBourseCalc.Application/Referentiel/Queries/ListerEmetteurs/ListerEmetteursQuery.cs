using CemacBourseCalc.Application.Referentiel.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteurs;

/// <summary>Tous les émetteurs, par nom. Quelques dizaines sur la BVMAC : pas de pagination.</summary>
public sealed record ListerEmetteursQuery : IRequest<IReadOnlyList<EmetteurResumeDto>>;
