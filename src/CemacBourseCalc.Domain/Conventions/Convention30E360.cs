namespace CemacBourseCalc.Domain.Conventions;

/// <summary>
/// 30E/360 (dite « européenne ») : chaque mois compte 30 jours ; un 31, en début
/// comme en fin de période, est ramené au 30. Différente du 30/360 américain,
/// qui ne ramène le 31 de fin qu'à condition que le début soit un 30 ou un 31.
/// </summary>
public sealed class Convention30E360 : IConventionDecompte
{
    public string Code => "30E_360";
    public string Libelle => "30E/360";
    public int JoursAnnee => 360;

    public int Jours(DateOnly debut, DateOnly fin)
    {
        var j1 = Math.Min(debut.Day, 30);
        var j2 = Math.Min(fin.Day, 30);
        return 360 * (fin.Year - debut.Year) + 30 * (fin.Month - debut.Month) + (j2 - j1);
    }
}
