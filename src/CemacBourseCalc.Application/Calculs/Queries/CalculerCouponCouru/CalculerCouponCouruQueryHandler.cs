using System.Globalization;
using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Place;
using MediatR;

namespace CemacBourseCalc.Application.Calculs.Queries.CalculerCouponCouru;

public sealed class CalculerCouponCouruQueryHandler(ILectureCalculs lecture)
    : IRequestHandler<CalculerCouponCouruQuery, Result<CalculCouponCouruDto>>
{
    /// <summary>Borne de lecture du calendrier : un délai de règlement ne dépasse jamais quelques semaines.</summary>
    private const int FenetreCalendrierJours = 60;
    private const int DelaiMaximum = 15;

    public async Task<Result<CalculCouponCouruDto>> Handle(CalculerCouponCouruQuery q, CancellationToken cancellationToken)
    {
        var obligation = await lecture.ObligationAsync(q.TitreId, cancellationToken);
        if (obligation is null)
            return Result<CalculCouponCouruDto>.Echec("Ce titre n'existe pas ou n'est pas une obligation.");

        var valeurDelai = await lecture.ParametreAsync(ClesParametres.DelaiReglement, cancellationToken);
        if (!int.TryParse(valeurDelai, NumberStyles.None, CultureInfo.InvariantCulture, out var delai) || delai > DelaiMaximum)
            return Result<CalculCouponCouruDto>.Echec("Le délai de règlement de la place n'est pas paramétré : le calcul est indisponible pour le moment.");

        var nonOuvres = await lecture.JoursNonOuvresAsync(
            q.DateNegociation, q.DateNegociation.AddDays(FenetreCalendrierJours), cancellationToken);
        var dateReglement = JoursOuvres.DateReglement(q.DateNegociation, delai, nonOuvres);

        try
        {
            ResultatCouponCouru cc;
            ResultatPrixPleinCoupon? prix = null;

            if (q.CoursPiedCoupon is { } cours)
            {
                prix = PrixPleinCoupon.Calculer(obligation, dateReglement, q.Quantite, cours);
                cc = prix.CouponCouru;
            }
            else
            {
                cc = CouponCouru.Calculer(obligation, dateReglement, q.Quantite);
            }

            return Result<CalculCouponCouruDto>.Succes(new CalculCouponCouruDto(
                obligation.Nom, q.DateNegociation, delai, dateReglement,
                cc.DebutPeriode, cc.FinPeriode, cc.Convention, cc.Jours, cc.JoursAnnee,
                cc.NominalRestant, cc.TauxFacial, cc.Quantite, cc.ParTitre, cc.Total,
                prix?.CoursPiedCoupon, prix?.PrixPiedCouponParTitre, prix?.PrixPiedCouponTotal, prix?.PrixPleinCouponTotal));
        }
        catch (DomainException e)
        {
            return Result<CalculCouponCouruDto>.Echec(e.Message);
        }
    }
}
