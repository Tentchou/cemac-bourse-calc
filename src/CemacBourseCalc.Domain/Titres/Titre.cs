using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>
/// Un titre coté à la BVMAC : ce que les obligations et les actions ont en commun. Montants en francs CFA (XAF).
/// L'ISIN et le type ne changent jamais : ce sont l'identité du titre (et la clé de l'import du bulletin).
/// </summary>
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
        VerifierCommun(nom, emetteurId, nominal, StatutTitre.Actif);

        Id = Identifiant.Nouveau();
        Isin = isin;
        TypeTitre = typeTitre;
        AppliquerCommun(nom, emetteurId, nominal, dateAdmission, StatutTitre.Actif);
    }

    /// <summary>
    /// Les règles communes, vérifiées AVANT tout changement : une modification refusée ne laisse
    /// jamais un titre à moitié modifié.
    /// </summary>
    protected static void VerifierCommun(string nom, Guid emetteurId, decimal nominal, StatutTitre statut)
    {
        if (string.IsNullOrWhiteSpace(nom)) throw new DomainException("Le nom du titre est obligatoire.");
        if (emetteurId == Guid.Empty) throw new DomainException("Choisissez l'émetteur du titre.");
        if (nominal <= 0m) throw new DomainException("La valeur nominale doit être supérieure à 0 FCFA.");
        if (!Enum.IsDefined(statut)) throw new DomainException("Le statut du titre n'est pas reconnu.");
    }

    /// <summary>Applique les champs communs, déjà vérifiés par <see cref="VerifierCommun"/>.</summary>
    protected void AppliquerCommun(string nom, Guid emetteurId, decimal nominal, DateOnly? dateAdmission, StatutTitre statut)
    {
        Nom = nom.Trim();
        EmetteurId = emetteurId;
        Nominal = nominal;
        DateAdmission = dateAdmission;
        Statut = statut;
    }

    public void ChangerStatut(StatutTitre statut)
    {
        if (!Enum.IsDefined(statut)) throw new DomainException("Le statut du titre n'est pas reconnu.");
        Statut = statut;
    }

    /// <summary>Retire le titre de la cote : il disparaît du site public, son historique est gardé.</summary>
    public void Radier() => Statut = StatutTitre.Radie;
}
