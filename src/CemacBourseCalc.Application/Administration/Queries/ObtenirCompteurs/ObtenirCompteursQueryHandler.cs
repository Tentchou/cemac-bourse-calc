using CemacBourseCalc.Application.Administration.Dtos;
using CemacBourseCalc.Application.Administration.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Queries.ObtenirCompteurs;

public sealed class ObtenirCompteursQueryHandler(ICompteursLecture lecture) : IRequestHandler<ObtenirCompteursQuery, CompteursAdminDto>
{
    public Task<CompteursAdminDto> Handle(ObtenirCompteursQuery q, CancellationToken cancellationToken) =>
        lecture.CompteursAsync(cancellationToken);
}
