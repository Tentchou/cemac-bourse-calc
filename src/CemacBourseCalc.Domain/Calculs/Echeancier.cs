using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Place;

namespace CemacBourseCalc.Domain.Calculs;

/// <summary>
/// Une échéance, pour la quantité détenue. <see cref="CouponParTitre"/> et <see cref="CapitalParTitre"/>
/// sont donnés pour l'explication ; les montants de la ligne sont calculés sur la quantité, arrondis une fois.
/// </summary>
public sealed record LigneEcheancier(
    int Rang,
    DateOnly DebutPeriode,
    DateOnly DateTheorique,
    DateOnly DatePaiement,
    bool PeriodeReguliere,
    decimal NominalDebutParTitre,
    decimal CouponParTitre,
    decimal CapitalParTitre,
    decimal NominalRestantApresParTitre,
    decimal Coupon,
    decimal Capital,
    decimal Total);

public sealed record ResultatEcheancier(
    int Quantite,
    DateOnly? FluxApresLe,
    IReadOnlyList<LigneEcheancier> Lignes,
    decimal TotalCoupons,
    decimal TotalCapital,
    decimal TotalFlux);

/// <summary>
/// Ce que le porteur reçoit, échéance par échéance : le coupon, le capital remboursé, et le total.
///
///   coupon d'une période régulière   = nominal restant × taux / nombre de coupons par an
///   coupon d'une période irrégulière = nominal restant × taux × fraction d'année (convention du titre)
///
/// Le nominal restant est celui du début de la période. Un paiement prévu un jour non ouvré est
/// reporté au jour ouvré suivant, sans changer son montant.
/// </summary>
public static class Echeancier
{
    /// <param name="fluxApresLe">
    /// Si renseignée (date de règlement d'un achat), seules les échéances strictement postérieures sont
    /// rendues : un coupon qui tombe le jour du règlement revient encore au vendeur.
    /// </param>
    public static ResultatEcheancier Calculer(
        CaracteristiquesObligation o, int quantite, IReadOnlySet<DateOnly> nonOuvres, DateOnly? fluxApresLe = null)
    {
        if (quantite <= 0) throw new DomainException("La quantité doit être d'au moins un titre.");
        if (fluxApresLe is { } apres && apres >= o.DateEcheance)
            throw new DomainException($"Le titre {o.Nom} arrive à échéance le {o.DateEcheance:dd/MM/yyyy} : il ne reste aucune échéance à percevoir.");

        var convention = o.Convention;
        var couponsParAn = o.Periodicite == Periodicite.InFine ? 0 : 12 / (int)o.Periodicite;
        var capitalParDate = Amortissement.Remboursements(o).ToDictionary(r => r.Date, r => r.Capital);

        var lignes = new List<LigneEcheancier>();
        var nominal = o.Nominal;

        foreach (var periode in CalendrierCoupons.Periodes(o))
        {
            var taux = o.TauxFacial / 100m;
            var couponExact = periode.Reguliere
                ? nominal * taux / couponsParAn
                : nominal * taux * convention.FractionAnnee(periode.Debut, periode.Fin);
            var capital = capitalParDate.GetValueOrDefault(periode.Fin);
            var nominalApres = nominal - capital;

            if (fluxApresLe is null || periode.Fin > fluxApresLe)
            {
                var coupon = Arrondis.Montant(couponExact * quantite);
                var capitalTotal = capital * quantite; // la part de capital est déjà au centime
                lignes.Add(new LigneEcheancier(
                    periode.Rang, periode.Debut, periode.Fin,
                    JoursOuvres.ReporterAuJourOuvreSuivant(periode.Fin, nonOuvres),
                    periode.Reguliere, nominal, Arrondis.Montant(couponExact), capital, nominalApres,
                    coupon, capitalTotal, coupon + capitalTotal));
            }

            nominal = nominalApres;
        }

        var totalCoupons = lignes.Sum(l => l.Coupon);
        var totalCapital = lignes.Sum(l => l.Capital);
        return new ResultatEcheancier(quantite, fluxApresLe, lignes, totalCoupons, totalCapital, totalCoupons + totalCapital);
    }
}
