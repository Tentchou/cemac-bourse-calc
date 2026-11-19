using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;

namespace CemacBourseCalc.Domain.Tests.Titres;

public class ObligationTests
{
    private static Isin IsinCameroun()
    {
        const string onze = "CM000000001";
        return Isin.Creer(onze + Isin.ChiffreControle(onze));
    }

    private static Obligation Creer(
        ModeRemboursement mode = ModeRemboursement.AmortissementConstant, int differe = 1,
        DateOnly? echeance = null, string convention = "EXACT_365") =>
        new(IsinCameroun(), "Emprunt d'exemple 6,50 % 2026-2031", "Émetteur d'exemple", 10_000m,
            6.5m, Periodicite.Annuelle, new DateOnly(2026, 6, 30), echeance ?? new DateOnly(2031, 6, 30),
            mode, differe, convention);

    [Fact]
    public void Une_obligation_valide_garde_ses_caracteristiques()
    {
        var o = Creer();
        Assert.Equal(PaysCemac.CM, o.Pays);
        Assert.Equal("Exact/365", o.Convention.Libelle);
        Assert.Equal(1, o.DiffereEcheances);
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

    [Fact]
    public void Une_convention_inconnue_est_refusee()
    {
        Assert.Throws<DomainException>(() => Creer(convention: "ACT_ACT"));
    }
}
