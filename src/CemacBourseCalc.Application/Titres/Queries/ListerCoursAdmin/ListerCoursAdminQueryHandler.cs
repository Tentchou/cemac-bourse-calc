using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Common;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ListerCoursAdmin;

/// <summary>La page demandée ; chaque cours d'obligation avec un coupon couru publié est contrôlé.</summary>
public sealed class ListerCoursAdminQueryHandler(IMarcheAdminLecture lecture, ILectureCalculs calculs)
    : IRequestHandler<ListerCoursAdminQuery, PageDto<CoursAdminDto>>
{
    public async Task<PageDto<CoursAdminDto>> Handle(ListerCoursAdminQuery q, CancellationToken cancellationToken)
    {
        var page = await lecture.CoursAsync(q, cancellationToken);
        var controles = await ControleCouponCouru.ControlerAsync(calculs, page.Lignes
            .Select(l => l.Caracteristiques is { } c && l.Cours.CouponCouruPublie is { } publie
                ? new ObligationAControler(c, l.Cours.DateSeance, publie)
                : null)
            .ToList(), cancellationToken);

        return new PageDto<CoursAdminDto>(
            page.Lignes.Select((l, i) => l.Cours with { Controle = controles[i] }).ToList(),
            page.Total, page.Page, page.Taille);
    }
}
