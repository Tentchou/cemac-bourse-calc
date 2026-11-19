using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerTitres;

public sealed class ListerTitresQueryHandler(ITitresLecture lecture)
    : IRequestHandler<ListerTitresQuery, IReadOnlyList<TitreResumeDto>>
{
    public Task<IReadOnlyList<TitreResumeDto>> Handle(ListerTitresQuery request, CancellationToken cancellationToken) =>
        lecture.ListerAsync(cancellationToken);
}
