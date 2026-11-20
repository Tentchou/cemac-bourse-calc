using CemacBourseCalc.Application.Calculs.Common;
using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Place;
using MediatR;

namespace CemacBourseCalc.Application.Calculs.Queries.CalculerRendementObligation;

public sealed class CalculerRendementObligationQueryHandler(ILectureCalculs lecture)
    : IRequestHandler<CalculerRendementObligationQuery, Result<RendementObligationDto>>
{
    /// <summary>Borne de lecture du calendrier : un délai de règlement ne dépasse jamais quelques semaines.</summary>
    private const int FenetreCalendrierJours = 60;

    public async Task<Result<RendementObligationDto>> Handle(CalculerRendementObligationQuery q, CancellationToken cancellationToken)
    {
        var obligation = await lecture.ObligationAsync(q.TitreId, cancellationToken);
        if (obligation is null)
            return Result<RendementObligationDto>.Echec("Ce titre n'existe pas ou n'est pas une obligation.");

        if (await ReglesPlace.DelaiReglementAsync(lecture, cancellationToken) is not { } delai)
            return Result<RendementObligationDto>.Echec(ReglesPlace.MessageDelaiAbsent);

        var nonOuvres = await lecture.JoursNonOuvresAsync(
            q.DateNegociation, q.DateNegociation.AddDays(FenetreCalendrierJours), cancellationToken);
        var dateReglement = JoursOuvres.DateReglement(q.DateNegociation, delai, nonOuvres);

        try
        {
            var r = RendementObligation.Calculer(obligation, dateReglement, q.CoursPiedCoupon);
            return Result<RendementObligationDto>.Succes(new RendementObligationDto(
                obligation.Nom, q.DateNegociation, delai, r.DateReglement, r.CoursPiedCoupon,
                r.NominalRestant, r.TauxFacial, r.PrixPiedCouponParTitre, r.CouponCouruParTitre,
                r.PrixPleinCouponParTitre, r.CouponAnnuelParTitre, r.RendementCourant, r.RendementActuariel,
                RendementObligation.JoursAnneeActuarielle,
                r.Flux.Select(f => new FluxActualiseDto(f.Date, f.Jours, f.FractionAnnee, f.Coupon, f.Capital,
                    f.Flux, f.FacteurActualisation, f.ValeurActuelle)).ToList()));
        }
        catch (DomainException e)
        {
            return Result<RendementObligationDto>.Echec(e.Message);
        }
    }
}
