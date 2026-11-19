using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>Un dividende versé par action, pour un exercice. Plusieurs versements possibles pour un même exercice (acompte).</summary>
public sealed class Dividende
{
    public Guid Id { get; private set; }
    public Guid TitreId { get; private set; }
    public int Exercice { get; private set; }
    public decimal MontantBrut { get; private set; }
    public decimal? MontantNet { get; private set; }
    public DateOnly? DateDetachement { get; private set; }
    public DateOnly? DatePaiement { get; private set; }

    private Dividende() { } // EF Core

    public Dividende(Guid titreId, int exercice, decimal montantBrut, decimal? montantNet, DateOnly? dateDetachement, DateOnly? datePaiement)
    {
        if (titreId == Guid.Empty) throw new DomainException("Le titre du dividende est obligatoire.");
        if (exercice is < 1990 or > 2100) throw new DomainException("L'exercice du dividende n'est pas valide.");
        if (montantBrut <= 0m) throw new DomainException("Le dividende brut doit être supérieur à 0 FCFA.");
        if (montantNet is { } net && (net <= 0m || net > montantBrut))
            throw new DomainException("Le dividende net doit être positif et au plus égal au brut.");
        if (dateDetachement is { } d && datePaiement is { } p && p < d)
            throw new DomainException("La date de paiement ne peut pas précéder la date de détachement.");

        Id = Identifiant.Nouveau();
        TitreId = titreId;
        Exercice = exercice;
        MontantBrut = montantBrut;
        MontantNet = montantNet;
        DateDetachement = dateDetachement;
        DatePaiement = datePaiement;
    }
}
