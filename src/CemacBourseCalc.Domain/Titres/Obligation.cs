using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Conventions;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>Une obligation à taux fixe, avec ses caractéristiques de prospectus.</summary>
public sealed class Obligation : Titre
{
    /// <summary>Taux facial annuel en pourcentage (6,5 pour 6,5 %).</summary>
    public decimal TauxFacial { get; private set; }
    public Periodicite Periodicite { get; private set; }
    public DateOnly DateJouissance { get; private set; }

    /// <summary>Renseignée seulement si le premier coupon ne tombe pas à jouissance + une période (coupon irrégulier).</summary>
    public DateOnly? DatePremierCoupon { get; private set; }

    public DateOnly DateEcheance { get; private set; }
    public ModeRemboursement ModeRemboursement { get; private set; }

    /// <summary>Nombre d'échéances de coupon sans remboursement de capital, au début de la vie du titre.</summary>
    public int DiffereEcheances { get; private set; }

    public string CodeConvention { get; private set; } = null!;
    public RegimeFiscal RegimeFiscal { get; private set; }

    public IConventionDecompte Convention =>
        ConventionsDecompte.Trouver(CodeConvention)
        ?? throw new DomainException($"La convention « {CodeConvention} » du titre {Nom} n'est pas connue.");

    private Obligation() { } // EF Core

    public Obligation(
        Isin isin, string nom, Guid emetteurId, decimal nominal, DateOnly? dateAdmission,
        decimal tauxFacial, Periodicite periodicite, DateOnly dateJouissance, DateOnly? datePremierCoupon, DateOnly dateEcheance,
        ModeRemboursement modeRemboursement, int differeEcheances, string codeConvention, RegimeFiscal regimeFiscal)
        : base(isin, nom, emetteurId, TypeTitre.Obligation, nominal, dateAdmission)
    {
        if (tauxFacial < 0m || tauxFacial > 100m) throw new DomainException("Le taux facial s'exprime en pourcentage, entre 0 et 100.");
        if (dateEcheance <= dateJouissance) throw new DomainException("La date d'échéance doit être après la date de jouissance.");
        if (datePremierCoupon is { } premier && (premier <= dateJouissance || premier > dateEcheance))
            throw new DomainException("La date du premier coupon doit être après la jouissance et au plus tard à l'échéance.");
        if (differeEcheances < 0) throw new DomainException("Le différé d'amortissement ne peut pas être négatif.");
        if (modeRemboursement == ModeRemboursement.InFine && differeEcheances > 0)
            throw new DomainException("Un différé d'amortissement n'a de sens que pour une obligation amortissable.");
        if (ConventionsDecompte.Trouver(codeConvention) is null)
            throw new DomainException($"La convention de décompte « {codeConvention} » n'est pas connue.");

        TauxFacial = tauxFacial;
        Periodicite = periodicite;
        DateJouissance = dateJouissance;
        DatePremierCoupon = datePremierCoupon;
        DateEcheance = dateEcheance;
        ModeRemboursement = modeRemboursement;
        DiffereEcheances = differeEcheances;
        CodeConvention = codeConvention.Trim().ToUpperInvariant();
        RegimeFiscal = regimeFiscal;
    }
}
