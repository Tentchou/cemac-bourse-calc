using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Referentiel;

/// <summary>Celui qui émet les titres : un État, une entreprise ou un organisme supranational de la CEMAC.</summary>
public sealed class Emetteur
{
    public Guid Id { get; private set; }
    public string Nom { get; private set; } = null!;
    public string? Sigle { get; private set; }
    public TypeEmetteur Type { get; private set; }
    public PaysCemac PaysCode { get; private set; }
    public string? SiteWeb { get; private set; }

    private Emetteur() { } // EF Core

    public Emetteur(string nom, string? sigle, TypeEmetteur type, PaysCemac paysCode, string? siteWeb)
    {
        if (string.IsNullOrWhiteSpace(nom)) throw new DomainException("Le nom de l'émetteur est obligatoire.");
        if (!string.IsNullOrWhiteSpace(siteWeb)
            && !(Uri.TryCreate(siteWeb.Trim(), UriKind.Absolute, out var adresse) && adresse.Scheme is "http" or "https"))
            throw new DomainException("Le site web de l'émetteur doit être une adresse complète (https://…).");

        Id = Identifiant.Nouveau();
        Nom = nom.Trim();
        Sigle = string.IsNullOrWhiteSpace(sigle) ? null : sigle.Trim();
        Type = type;
        PaysCode = paysCode;
        SiteWeb = string.IsNullOrWhiteSpace(siteWeb) ? null : siteWeb.Trim();
    }
}
