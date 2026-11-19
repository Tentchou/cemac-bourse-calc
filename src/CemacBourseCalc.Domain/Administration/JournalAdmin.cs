using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Administration;

/// <summary>
/// Une action de l'administration : qui, quand, sur quoi, et l'état avant / après (JSON).
/// L'identifiant de l'entité est un texte : un GUID pour un titre, une date pour un jour non ouvré, une clé pour un paramètre.
/// </summary>
public sealed class JournalAdmin
{
    public Guid Id { get; private set; }
    public Guid AdministrateurId { get; private set; }
    public DateTimeOffset Date { get; private set; }
    public string Action { get; private set; } = null!;
    public string Entite { get; private set; } = null!;
    public string EntiteId { get; private set; } = null!;
    public string? Avant { get; private set; }
    public string? Apres { get; private set; }

    private JournalAdmin() { } // EF Core

    public JournalAdmin(Guid administrateurId, DateTimeOffset date, string action, string entite, string entiteId, string? avant, string? apres)
    {
        if (administrateurId == Guid.Empty) throw new DomainException("L'auteur de l'action est obligatoire.");
        if (string.IsNullOrWhiteSpace(action) || string.IsNullOrWhiteSpace(entite) || string.IsNullOrWhiteSpace(entiteId))
            throw new DomainException("L'action, l'entité et son identifiant sont obligatoires.");

        Id = Identifiant.Nouveau();
        AdministrateurId = administrateurId;
        Date = Horodatage.EnUtc(date);
        Action = action.Trim();
        Entite = entite.Trim();
        EntiteId = entiteId.Trim();
        Avant = avant;
        Apres = apres;
    }
}
