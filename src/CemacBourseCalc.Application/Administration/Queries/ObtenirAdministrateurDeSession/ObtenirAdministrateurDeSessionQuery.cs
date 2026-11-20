using CemacBourseCalc.Application.Administration.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Queries.ObtenirAdministrateurDeSession;

/// <summary>Qui est connecté avec ce jeton ? null si la session n'est pas (ou plus) valide.</summary>
public sealed record ObtenirAdministrateurDeSessionQuery(string Jeton) : IRequest<AdministrateurConnecteDto?>
{
    public override string ToString() => nameof(ObtenirAdministrateurDeSessionQuery);
}
