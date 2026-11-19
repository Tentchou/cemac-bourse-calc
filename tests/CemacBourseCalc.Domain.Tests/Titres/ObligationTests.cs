using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;

namespace CemacBourseCalc.Domain.Tests.Titres;

public class ObligationTests
{
    private static readonly Guid Emetteur = Guid.CreateVersion7();

    private static Isin IsinCameroun()
    {
        const string onze = "CM000000001";
        return Isin.Creer(onze + Isin.ChiffreControle(onze));
    }

    private static Obligation Creer(
        ModeRemboursement mode = ModeRemboursement.AmortissementConstant, int differe = 1,
        DateOnly? premierCoupon = null, DateOnly? echeance = null, string convention = "EXACT_365", Guid? emetteur = null) =>
        new(IsinCameroun(), "Emprunt d'exemple 6,50 % 2026-2031", emetteur ?? Emetteur, 10_000m, null,
            6.5m, Periodicite.Annuelle, new DateOnly(2026, 6, 30), premierCoupon, echeance ?? new DateOnly(2031, 6, 30),
            mode, differe, convention, RegimeFiscal.Net);

    [Fact]
    public void Une_obligation_valide_garde_ses_caracteristiques()
    {
        var o = Creer();
        Assert.NotEqual(Guid.Empty, o.Id);
        Assert.Equal(7, o.Id.Version); // GUID version 7, ordonné dans le temps
        Assert.Equal(TypeTitre.Obligation, o.TypeTitre);
        Assert.Equal(StatutTitre.Actif, o.Statut);
        Assert.Equal("Exact/365", o.Convention.Libelle);
        Assert.Equal(PaysCemac.CM, o.Isin.Pays);
    }

    [Fact]
    public void Deux_obligations_ont_des_identifiants_differents()
    {
        Assert.NotEqual(Creer().Id, Creer().Id);
    }

    [Fact]
    public void Un_differe_sur_une_obligation_in_fine_est_refuse()
    {
        Assert.Throws<DomainException>(() => Creer(ModeRemboursement.InFine, differe: 1));
    }

    [Fact]
    public void Une_echeance_avant_la_jouissance_est_refusee()
    {
        Assert.Throws<DomainException>(() => Creer(echeance: new DateOnly(2026, 6, 30)));
    }

    [Theory]
    [InlineData(2026, 6, 30)] // le jour de la jouissance
    [InlineData(2031, 7, 1)]  // après l'échéance
    public void Un_premier_coupon_hors_de_la_vie_du_titre_est_refuse(int a, int m, int j)
    {
        Assert.Throws<DomainException>(() => Creer(premierCoupon: new DateOnly(a, m, j)));
    }

    [Fact]
    public void Un_premier_coupon_irregulier_est_accepte()
    {
        Assert.Equal(new DateOnly(2027, 3, 31), Creer(premierCoupon: new DateOnly(2027, 3, 31)).DatePremierCoupon);
    }

    [Fact]
    public void Une_convention_inconnue_est_refusee()
    {
        Assert.Throws<DomainException>(() => Creer(convention: "ACT_ACT"));
    }

    [Fact]
    public void Un_titre_sans_emetteur_est_refuse()
    {
        Assert.Throws<DomainException>(() => Creer(emetteur: Guid.Empty));
    }
}
