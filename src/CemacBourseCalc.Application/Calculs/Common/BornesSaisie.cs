namespace CemacBourseCalc.Application.Calculs.Common;

/// <summary>Bornes de saisie communes aux calculs publics : elles protègent le serveur, pas une règle de marché.</summary>
public static class BornesSaisie
{
    public const int QuantiteMaximum = 1_000_000_000;
    public static readonly DateOnly DateMinimum = new(1990, 1, 1);
    public static readonly DateOnly DateMaximum = new(2100, 12, 31);
}
