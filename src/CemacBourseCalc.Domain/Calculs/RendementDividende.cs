using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Calculs;

public sealed record ResultatRendementDividende(
    decimal Cours,
    int Exercice,
    decimal DividendeBrut,
    decimal? DividendeNet,
    decimal RendementBrut,
    decimal? RendementNet);

/// <summary>
/// Rendement du dividende d'une action, sur le dernier exercice connu :
///
///   rendement = dividende par action de l'exercice / cours × 100
///
/// Le dividende de l'exercice est la somme de ses versements (acompte et solde). Le cours d'une
/// action s'exprime en FCFA. C'est un rendement passé : rien ne garantit le prochain dividende.
/// </summary>
public static class RendementDividende
{
    public const decimal CoursMaximum = 100_000_000m;

    public static ResultatRendementDividende Calculer(decimal cours, int exercice, decimal dividendeBrut, decimal? dividendeNet)
    {
        if (cours <= 0m || cours > CoursMaximum)
            throw new DomainException("Le cours d'une action s'exprime en FCFA et doit être supérieur à 0.");
        if (decimal.Round(cours, 2) != cours)
            throw new DomainException("Le cours accepte au plus 2 décimales.");
        if (dividendeBrut <= 0m)
            throw new DomainException("Le dividende brut doit être supérieur à 0 FCFA.");
        if (dividendeNet is { } n && (n <= 0m || n > dividendeBrut))
            throw new DomainException("Le dividende net doit être positif et au plus égal au brut.");

        return new ResultatRendementDividende(
            cours, exercice, dividendeBrut, dividendeNet,
            Pourcentage(dividendeBrut, cours),
            dividendeNet is { } net ? Pourcentage(net, cours) : null);
    }

    private static decimal Pourcentage(decimal dividende, decimal cours) =>
        decimal.Round(dividende / cours * 100m, 4, MidpointRounding.AwayFromZero);
}
