using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>Le document public d'où viennent les chiffres d'une fiche (prospectus, avis, bulletin de cote).</summary>
public sealed class DocumentSource
{
    public Guid Id { get; private set; }
    public Guid TitreId { get; private set; }
    public TypeDocument Type { get; private set; }
    public string Libelle { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public DateOnly? DatePublication { get; private set; }

    private DocumentSource() { } // EF Core

    public DocumentSource(Guid titreId, TypeDocument type, string libelle, string url, DateOnly? datePublication)
    {
        if (titreId == Guid.Empty) throw new DomainException("Le titre du document est obligatoire.");
        if (string.IsNullOrWhiteSpace(libelle)) throw new DomainException("Le libellé du document est obligatoire.");
        if (!(Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var adresse) && adresse.Scheme is "http" or "https"))
            throw new DomainException("L'adresse du document doit être complète (https://…).");

        Id = Identifiant.Nouveau();
        TitreId = titreId;
        Type = type;
        Libelle = libelle.Trim();
        Url = adresse.ToString();
        DatePublication = datePublication;
    }
}
