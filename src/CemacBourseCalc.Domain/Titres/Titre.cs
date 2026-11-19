using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>Un titre coté à la BVMAC. Les montants sont en francs CFA (XAF).</summary>
public abstract class Titre
{
    public int Id { get; private set; }
    public Isin Isin { get; private set; } = null!;
    public string Nom { get; private set; } = null!;
    public string Emetteur { get; private set; } = null!;
    public decimal Nominal { get; private set; }

    /// <summary>Pays de l'émetteur, lu sur l'ISIN.</summary>
    public PaysCemac Pays => Isin.Pays;

    protected Titre() { } // EF Core

    protected Titre(Isin isin, string nom, string emetteur, decimal nominal)
    {
        if (string.IsNullOrWhiteSpace(nom)) throw new DomainException("Le nom du titre est obligatoire.");
        if (string.IsNullOrWhiteSpace(emetteur)) throw new DomainException("L'émetteur du titre est obligatoire.");
        if (nominal <= 0m) throw new DomainException("La valeur nominale doit être supérieure à 0 FCFA.");

        Isin = isin;
        Nom = nom.Trim();
        Emetteur = emetteur.Trim();
        Nominal = nominal;
    }
}
