using CemacBourseCalc.Application.Bulletins.Dtos;
using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Bulletins.Queries.AnalyserBulletin;

/// <summary>Lit un bulletin de la cote (PDF) et le rapproche de la base. Rien n'est enregistré.</summary>
public sealed record AnalyserBulletinQuery(byte[] Contenu) : IRequest<Result<BulletinAnalyseDto>>
{
    /// <summary>Un bulletin fait environ 200 Ko ; 10 Mo laissent une large marge.</summary>
    public const int TailleMaximum = 10 * 1024 * 1024;

    /// <summary>Un bulletin fait une trentaine de pages.</summary>
    public const int PagesMaximum = 100;
}
