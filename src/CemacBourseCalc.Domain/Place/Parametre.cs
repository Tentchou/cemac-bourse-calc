using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Place;

/// <summary>Un réglage de la place, modifiable par l'administration (rien n'est écrit en dur dans le code).</summary>
public sealed class Parametre
{
    public string Cle { get; private set; } = null!;
    public string Valeur { get; private set; } = null!;
    public string Libelle { get; private set; } = null!;
    public DateTimeOffset? ModifieLe { get; private set; }
    public Guid? ModifieParId { get; private set; }

    private Parametre() { } // EF Core

    public Parametre(string cle, string valeur, string libelle)
    {
        if (string.IsNullOrWhiteSpace(cle)) throw new DomainException("La clé du paramètre est obligatoire.");
        if (string.IsNullOrWhiteSpace(libelle)) throw new DomainException("Le libellé du paramètre est obligatoire.");
        Cle = cle.Trim().ToUpperInvariant();
        Valeur = valeur?.Trim() ?? string.Empty;
        Libelle = libelle.Trim();
    }

    public void Modifier(string valeur, Guid administrateurId, DateTimeOffset instant)
    {
        Valeur = valeur?.Trim() ?? string.Empty;
        ModifieParId = administrateurId;
        ModifieLe = Horodatage.EnUtc(instant);
    }
}

/// <summary>Les clés connues et leur valeur initiale (créées par la migration).</summary>
public static class ClesParametres
{
    public const string DelaiReglement = "DELAI_REGLEMENT_JOURS_OUVRES";
    public const string RegleReport = "REGLE_REPORT_PAIEMENT";

    public static IReadOnlyList<Parametre> ValeursInitiales { get; } =
    [
        new(DelaiReglement, "3", "Délai de règlement-livraison du marché coté, en jours ouvrés (T+N)"),
        new(RegleReport, "JOUR_OUVRE_SUIVANT", "Report d'un paiement qui tombe un jour non ouvré")
    ];
}
