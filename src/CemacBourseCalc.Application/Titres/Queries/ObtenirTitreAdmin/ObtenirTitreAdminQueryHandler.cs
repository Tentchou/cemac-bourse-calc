using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ObtenirTitreAdmin;

public sealed class ObtenirTitreAdminQueryHandler(ITitresAdminLecture lecture)
    : IRequestHandler<ObtenirTitreAdminQuery, TitreAdminDetailDto?>
{
    public Task<TitreAdminDetailDto?> Handle(ObtenirTitreAdminQuery request, CancellationToken cancellationToken) =>
        lecture.DetailAsync(request.Id, cancellationToken);
}
