using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Administration;

/// <summary>Un compte de l'administration. Le mot de passe n'est jamais stocké : seulement son empreinte (hash).</summary>
public sealed class Administrateur
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string Nom { get; private set; } = null!;
    public string MotDePasseHash { get; private set; } = null!;
    public RoleAdministrateur Role { get; private set; }
    public bool Actif { get; private set; }
    public DateTimeOffset CreeLe { get; private set; }
    public DateTimeOffset? DerniereConnexion { get; private set; }

    private Administrateur() { } // EF Core

    public Administrateur(string email, string nom, string motDePasseHash, RoleAdministrateur role, DateTimeOffset creeLe)
    {
        var e = email?.Trim().ToLowerInvariant() ?? string.Empty;
        if (e.Length < 3 || !e.Contains('@')) throw new DomainException("L'adresse e-mail de l'administrateur n'est pas valide.");
        if (string.IsNullOrWhiteSpace(nom)) throw new DomainException("Le nom de l'administrateur est obligatoire.");
        if (string.IsNullOrWhiteSpace(motDePasseHash)) throw new DomainException("Le mot de passe est obligatoire.");

        Id = Identifiant.Nouveau();
        Email = e;
        Nom = nom.Trim();
        MotDePasseHash = motDePasseHash;
        Role = role;
        Actif = true;
        CreeLe = Horodatage.EnUtc(creeLe);
    }

    public void NoterConnexion(DateTimeOffset instant) => DerniereConnexion = Horodatage.EnUtc(instant);
    public void Desactiver() => Actif = false;
    public void ChangerMotDePasse(string nouveauHash) =>
        MotDePasseHash = string.IsNullOrWhiteSpace(nouveauHash) ? throw new DomainException("Le mot de passe est obligatoire.") : nouveauHash;
}
