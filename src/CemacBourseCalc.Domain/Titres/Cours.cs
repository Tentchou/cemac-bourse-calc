using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>
/// Une ligne du bulletin de cote de la BVMAC pour une séance. Obligations : cours en % du nominal,
/// pied de coupon. Actions : cours en FCFA. Le coupon couru publié sert à contrôler notre calcul.
/// </summary>
public sealed class Cours
{
    public Guid Id { get; private set; }
    public Guid TitreId { get; private set; }
    public DateOnly DateSeance { get; private set; }
    public decimal? CoursReference { get; private set; }
    public decimal? CoursCloture { get; private set; }
    public long? Volume { get; private set; }
    public decimal? CouponCouruPublie { get; private set; }

    private Cours() { } // EF Core

    public Cours(Guid titreId, DateOnly dateSeance, decimal? coursReference, decimal? coursCloture, long? volume, decimal? couponCouruPublie)
    {
        if (titreId == Guid.Empty) throw new DomainException("Le titre du cours est obligatoire.");
        if (coursReference is null && coursCloture is null) throw new DomainException("Saisissez au moins un cours (référence ou clôture).");
        if (coursReference <= 0m || coursCloture <= 0m) throw new DomainException("Un cours doit être supérieur à 0.");
        if (volume < 0) throw new DomainException("Le volume ne peut pas être négatif.");
        if (couponCouruPublie < 0m) throw new DomainException("Le coupon couru ne peut pas être négatif.");

        Id = Identifiant.Nouveau();
        TitreId = titreId;
        DateSeance = dateSeance;
        CoursReference = coursReference;
        CoursCloture = coursCloture;
        Volume = volume;
        CouponCouruPublie = couponCouruPublie;
    }
}
