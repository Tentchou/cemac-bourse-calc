using CemacBourseCalc.Application.Calculs.Common;
using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Place;
using MediatR;

namespace CemacBourseCalc.Application.Calculs.Queries.ObtenirEcheancier;

public sealed class ObtenirEcheancierQueryHandler(ILectureCalculs lecture)
    : IRequestHandler<ObtenirEcheancierQuery, Result<EcheancierDto>>
{
    /// <summary>Marge de lecture du calendrier après l'échéance : un report ne dépasse jamais quelques jours.</summary>
    private const int MargeCalendrierJours = 30;

    public async Task<Result<EcheancierDto>> Handle(ObtenirEcheancierQuery q, CancellationToken cancellationToken)
    {
        var obligation = await lecture.ObligationAsync(q.TitreId, cancellationToken);
        if (obligation is null)
            return Result<EcheancierDto>.Echec("Ce titre n'existe pas ou n'est pas une obligation.");

        if (!await ReglesPlace.ReportPrisEnChargeAsync(lecture, cancellationToken))
            return Result<EcheancierDto>.Echec(ReglesPlace.MessageReportInconnu);

        var debut = q.DateNegociation is { } negociation && negociation < obligation.DateJouissance ? negociation : obligation.DateJouissance;
        var nonOuvres = await lecture.JoursNonOuvresAsync(debut, obligation.DateEcheance.AddDays(MargeCalendrierJours), cancellationToken);

        DateOnly? dateReglement = null;
        if (q.DateNegociation is { } dateNegociation)
        {
            if (await ReglesPlace.DelaiReglementAsync(lecture, cancellationToken) is not { } delai)
                return Result<EcheancierDto>.Echec(ReglesPlace.MessageDelaiAbsent);
            dateReglement = JoursOuvres.DateReglement(dateNegociation, delai, nonOuvres);
        }

        try
        {
            var r = Echeancier.Calculer(obligation, q.Quantite, nonOuvres, dateReglement);

            return Result<EcheancierDto>.Succes(new EcheancierDto(
                obligation.Nom, obligation.Convention.Libelle, r.Quantite, q.DateNegociation, dateReglement,
                r.Lignes.Select(l => new LigneEcheancierDto(
                    l.Rang, l.DebutPeriode, l.DateTheorique, l.DatePaiement, l.PeriodeReguliere,
                    l.NominalDebutParTitre, l.CouponParTitre, l.CapitalParTitre, l.NominalRestantApresParTitre,
                    l.Coupon, l.Capital, l.Total)).ToList(),
                r.TotalCoupons, r.TotalCapital, r.TotalFlux));
        }
        catch (DomainException e)
        {
            return Result<EcheancierDto>.Echec(e.Message);
        }
    }
}
