using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Place;

/// <summary>Un jour sans séance à la BVMAC (férié, fermeture). La date est la clé : un jour n'est saisi qu'une fois.</summary>
public sealed class JourNonOuvre
{
    public DateOnly Date { get; private set; }
    public string Libelle { get; private set; } = null!;

    private JourNonOuvre() { } // EF Core

    public JourNonOuvre(DateOnly date, string libelle)
    {
        if (string.IsNullOrWhiteSpace(libelle)) throw new DomainException("Indiquez le libellé du jour non ouvré (ex. « Noël »).");
        Date = date;
        Libelle = libelle.Trim();
    }
}
