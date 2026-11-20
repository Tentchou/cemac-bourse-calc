namespace CemacBourseCalc.Application.Administration.Common;

/// <summary>Longueur plutôt que complexité imposée : une phrase de passe de 12 caractères ou plus.</summary>
public static class ReglesMotDePasse
{
    public const int LongueurMinimum = 12;
    public const int LongueurMaximum = 128;
}
