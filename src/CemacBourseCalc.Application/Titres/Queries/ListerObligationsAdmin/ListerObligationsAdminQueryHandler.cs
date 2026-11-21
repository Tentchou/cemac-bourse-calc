using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Common;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerObligationsAdmin;

/// <summary>La page demandée, chaque obligation contrôlée sur son dernier coupon couru publié.</summary>
public sealed class ListerObligationsAdminQueryHandler(ITitresAdminLecture lecture, ILectureCalculs calculs)
    : IRequestHandler<ListerObligationsAdminQuery, PageDto<ObligationAdminDto>>
{
    public async Task<PageDto<ObligationAdminDto>> Handle(ListerObligationsAdminQuery q, CancellationToken cancellationToken)
    {
        var page = await lecture.ObligationsAsync(q, cancellationToken);
        var controles = await ControleCouponCouru.ControlerAsync(calculs, page.Lignes
            .Select(l => l.DateSeance is { } seance && l.CouponPublie is { } publie
                ? new ObligationAControler(l.Caracteristiques, seance, publie)
                : null)
            .ToList(), cancellationToken);

        return new PageDto<ObligationAdminDto>(
            page.Lignes.Select((l, i) => l.Obligation with { Controle = controles[i] }).ToList(),
            page.Total, page.Page, page.Taille);
    }
}
