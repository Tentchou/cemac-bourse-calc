using CemacBourseCalc.Application.Referentiel.Dtos;
using CemacBourseCalc.Application.Referentiel.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteurs;

public sealed class ListerEmetteursQueryHandler(IEmetteursLecture lecture)
    : IRequestHandler<ListerEmetteursQuery, IReadOnlyList<EmetteurResumeDto>>
{
    public Task<IReadOnlyList<EmetteurResumeDto>> Handle(ListerEmetteursQuery q, CancellationToken cancellationToken) =>
        lecture.ListerAsync(cancellationToken);
}
