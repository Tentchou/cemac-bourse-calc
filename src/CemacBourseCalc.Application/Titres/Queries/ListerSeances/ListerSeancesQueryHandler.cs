using CemacBourseCalc.Application.Titres.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerSeances;

public sealed class ListerSeancesQueryHandler(IMarcheAdminLecture lecture) : IRequestHandler<ListerSeancesQuery, IReadOnlyList<DateOnly>>
{
    public Task<IReadOnlyList<DateOnly>> Handle(ListerSeancesQuery q, CancellationToken cancellationToken) =>
        lecture.SeancesAsync(ListerSeancesQuery.Nombre, cancellationToken);
}
