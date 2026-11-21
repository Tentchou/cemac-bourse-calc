using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerSeances;

/// <summary>Les dernières séances saisies (au plus 250, environ une année de bourse), pour le filtre des cours.</summary>
public sealed record ListerSeancesQuery : IRequest<IReadOnlyList<DateOnly>>
{
    public const int Nombre = 250;
}
