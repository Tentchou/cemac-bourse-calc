namespace CemacBourseCalc.Domain.Conventions;

/// <summary>
/// Convention de décompte des jours (« day count ») : comment compter les jours
/// d'une période et par quoi les diviser. La convention d'un titre est fixée par
/// son prospectus ; elle ne se déduit jamais.
/// </summary>
public interface IConventionDecompte
{
    /// <summary>Code stable, enregistré en base sur le titre (ex. « EXACT_365 »).</summary>
    string Code { get; }

    /// <summary>Libellé affiché (ex. « Exact/365 »).</summary>
    string Libelle { get; }

    /// <summary>Nombre de jours de l'année de référence (le diviseur).</summary>
    int JoursAnnee { get; }

    /// <summary>Jours comptés entre <paramref name="debut"/> (exclu) et <paramref name="fin"/> (inclus).</summary>
    int Jours(DateOnly debut, DateOnly fin);

    /// <summary>Fraction d'année = jours / jours de l'année, sans arrondi.</summary>
    decimal FractionAnnee(DateOnly debut, DateOnly fin) => (decimal)Jours(debut, fin) / JoursAnnee;
}
