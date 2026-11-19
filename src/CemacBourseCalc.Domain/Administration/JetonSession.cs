using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Administration;

/// <summary>Une session ouverte. Seule l'empreinte du jeton est stockée ; la révocation ferme la session à distance.</summary>
public sealed class JetonSession
{
    public Guid Id { get; private set; }
    public Guid AdministrateurId { get; private set; }
    public string JetonHash { get; private set; } = null!;
    public DateTimeOffset CreeLe { get; private set; }
    public DateTimeOffset ExpireLe { get; private set; }
    public DateTimeOffset? RevoqueLe { get; private set; }

    private JetonSession() { } // EF Core

    public JetonSession(Guid administrateurId, string jetonHash, DateTimeOffset creeLe, DateTimeOffset expireLe)
    {
        if (administrateurId == Guid.Empty) throw new DomainException("La session doit appartenir à un administrateur.");
        if (string.IsNullOrWhiteSpace(jetonHash)) throw new DomainException("Le jeton de session est obligatoire.");
        if (expireLe <= creeLe) throw new DomainException("Une session doit expirer après sa création.");

        Id = Identifiant.Nouveau();
        AdministrateurId = administrateurId;
        JetonHash = jetonHash;
        CreeLe = Horodatage.EnUtc(creeLe);
        ExpireLe = Horodatage.EnUtc(expireLe);
    }

    public bool EstValide(DateTimeOffset maintenant) => RevoqueLe is null && maintenant < ExpireLe;

    public void Revoquer(DateTimeOffset instant) => RevoqueLe ??= Horodatage.EnUtc(instant);
}
