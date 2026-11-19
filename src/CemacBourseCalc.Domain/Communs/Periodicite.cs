namespace CemacBourseCalc.Domain.Communs;

/// <summary>Périodicité des coupons. La valeur est le nombre de mois entre deux coupons (0 = un seul, à l'échéance).</summary>
public enum Periodicite
{
    InFine = 0,
    Mensuelle = 1,
    Trimestrielle = 3,
    Semestrielle = 6,
    Annuelle = 12
}
