namespace CemacBourseCalc.Domain.Calculs;

/// <summary>
/// Les montants s'arrondissent au centime, au plus proche (0,005 monte), comme le coupon couru
/// publié au bulletin de la BVMAC. On n'arrondit qu'à la fin d'un calcul, jamais en chemin.
/// </summary>
public static class Arrondis
{
    public const int DecimalesMontant = 2;

    public static decimal Montant(decimal valeur) =>
        Math.Round(valeur, DecimalesMontant, MidpointRounding.AwayFromZero);
}
