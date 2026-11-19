using CemacBourseCalc.Application.Titres.Dtos;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerTitres;

public sealed record ListerTitresQuery : IRequest<IReadOnlyList<TitreResumeDto>>;
