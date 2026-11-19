using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Calculs;

/// <summary>Le capital remboursé à une date de coupon, et ce qui reste dû ensuite (par titre).</summary>
public sealed record RemboursementCapital(DateOnly Date, decimal Capital, decimal NominalRestantApres);

/// <summary>
/// Le remboursement du capital d'un titre.
///
/// In fine : tout le nominal à l'échéance.
/// Amortissement constant : après les échéances de différé, le nominal est remboursé par parts
/// égales à chaque date de coupon restante. La part est arrondie au centime ; la dernière
/// échéance prend le reste, pour que la somme fasse exactement le nominal.
/// </summary>
public static class Amortissement
{
    public static IReadOnlyList<RemboursementCapital> Remboursements(CaracteristiquesObligation o)
    {
        var dates = CalendrierCoupons.Dates(o);

        if (o.ModeRemboursement == ModeRemboursement.InFine)
            return [new RemboursementCapital(o.DateEcheance, o.Nominal, 0m)];

        var nombre = dates.Count - o.DiffereEcheances;
        if (nombre <= 0)
            throw new DomainException(
                $"Le titre {o.Nom} a {dates.Count} échéance(s) pour un différé de {o.DiffereEcheances} : aucune échéance ne rembourse le capital. Vérifiez le prospectus.");

        var part = Arrondis.Montant(o.Nominal / nombre);
        var restant = o.Nominal;
        var resultat = new List<RemboursementCapital>(nombre);

        foreach (var date in dates.Skip(o.DiffereEcheances))
        {
            var capital = date == o.DateEcheance ? restant : part;
            restant -= capital;
            resultat.Add(new RemboursementCapital(date, capital, restant));
        }

        return resultat;
    }

    /// <summary>
    /// Le nominal restant dû pendant la période qui commence à <paramref name="debutPeriode"/> :
    /// le nominal moins tout ce qui a été remboursé jusqu'à cette date incluse.
    /// </summary>
    public static decimal NominalRestant(CaracteristiquesObligation o, DateOnly debutPeriode) =>
        o.Nominal - Remboursements(o).Where(r => r.Date <= debutPeriode).Sum(r => r.Capital);
}
