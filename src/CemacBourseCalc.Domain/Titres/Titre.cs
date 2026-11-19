using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>Un titre coté à la BVMAC : ce que les obligations et les actions ont en commun. Montants en francs CFA (XAF).</summary>
public abstract class Titre
{
    public Guid Id { get; private set; }
    public Isin Isin { get; private set; } = null!;
    public string Nom { get; private set; } = null!;
    public Guid EmetteurId { get; private set; }
    public TypeTitre TypeTitre { get; private set; }
    public decimal Nominal { get; private set; }
    public DateOnly? DateAdmission { get; private set; }
    public StatutTitre Statut { get; private set; }

    protected Titre() { } // EF Core

    protected Titre(Isin isin, string nom, Guid emetteurId, TypeTitre typeTitre, decimal nominal, DateOnly? dateAdmission)
    {
        if (string.IsNullOrWhiteSpace(nom)) throw new DomainException("Le nom du titre est obligatoire.");
        if (emetteurId == Guid.Empty) throw new DomainException("Choisissez l'émetteur du titre.");
        if (nominal <= 0m) throw new DomainException("La valeur nominale doit être supérieure à 0 FCFA.");

        Id = Identifiant.Nouveau();
        Isin = isin;
        Nom = nom.Trim();
        EmetteurId = emetteurId;
        TypeTitre = typeTitre;
        Nominal = nominal;
        DateAdmission = dateAdmission;
        Statut = StatutTitre.Actif;
    }

    public void ChangerStatut(StatutTitre statut) => Statut = statut;
}
