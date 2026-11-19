using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Tests.Calculs;

/// <summary>Obligations fictives des tests. Les valeurs sont choisies pour être recomptées à la main.</summary>
internal static class Exemples
{
    public static DateOnly D(int a, int m, int j) => new(a, m, j);

    /// <summary>10 000 FCFA, 6,50 %, coupon annuel au 30/06, in fine au 30/06/2030, Exact/365.</summary>
    public static CaracteristiquesObligation InFineExact365() => new(
        "Exemple in fine 6,50 % 2025-2030", 10_000m, 6.5m, Periodicite.Annuelle,
        D(2025, 6, 30), null, D(2030, 6, 30), ModeRemboursement.InFine, 0, "EXACT_365");

    /// <summary>
    /// 10 000 FCFA, 6,50 %, coupon annuel au 30/06, 5 échéances (2026 à 2030), 1 an de différé :
    /// 4 remboursements de 2 500 FCFA, de 2027 à 2030. Exact/360.
    /// </summary>
    public static CaracteristiquesObligation AmortissableExact360() => new(
        "Exemple amortissable 6,50 % 2025-2030", 10_000m, 6.5m, Periodicite.Annuelle,
        D(2025, 6, 30), null, D(2030, 6, 30), ModeRemboursement.AmortissementConstant, 1, "EXACT_360");

    /// <summary>
    /// 10 000 FCFA, 5 %, coupon semestriel, jouissance 15/01/2026, premier coupon irrégulier
    /// (long) au 30/09/2026, puis tous les six mois jusqu'au 30/09/2028. 30E/360.
    /// </summary>
    public static CaracteristiquesObligation PremierCouponIrregulier30E360() => new(
        "Exemple semestriel 5 % 2026-2028", 10_000m, 5m, Periodicite.Semestrielle,
        D(2026, 1, 15), D(2026, 9, 30), D(2028, 9, 30), ModeRemboursement.InFine, 0, "30E_360");
}
