using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using Action = CemacBourseCalc.Domain.Titres.Action;

namespace CemacBourseCalc.Domain.Tests.Titres;

/// <summary>Modifier et radier un titre : mêmes règles qu'à la création, jamais de titre à moitié modifié.</summary>
public class ModificationTitreTests
{
    private static readonly Guid EmetteurA = Guid.CreateVersion7();
    private static readonly Guid EmetteurB = Guid.CreateVersion7();

    private static Isin IsinCameroun()
    {
        const string onze = "CM000000001";
        return Isin.Creer(onze + Isin.ChiffreControle(onze));
    }

    private static Obligation Obligation() =>
        new(IsinCameroun(), "Emprunt 6,50 % 2026-2031", EmetteurA, 10_000m, null,
            6.5m, Periodicite.Annuelle, new DateOnly(2026, 6, 30), null, new DateOnly(2031, 6, 30),
            ModeRemboursement.InFine, 0, "EXACT_365", RegimeFiscal.Net);

    private static Action Action() => new(IsinCameroun(), "Société Exemple SA", EmetteurA, 10_000m, null, 1_000_000);

    [Fact]
    public void Une_obligation_modifiee_garde_son_identite_et_prend_les_nouvelles_valeurs()
    {
        var o = Obligation();
        var (id, isin) = (o.Id, o.Isin);

        o.Modifier(" Emprunt 6,75 % 2026-2031 ", EmetteurB, 10_000m, new DateOnly(2026, 7, 2), StatutTitre.Suspendu,
            6.75m, Periodicite.Semestrielle, new DateOnly(2026, 6, 30), new DateOnly(2026, 12, 30), new DateOnly(2031, 6, 30),
            ModeRemboursement.AmortissementConstant, 2, "exact_360", RegimeFiscal.Brut);

        Assert.Equal(id, o.Id);
        Assert.Equal(isin, o.Isin);
        Assert.Equal(TypeTitre.Obligation, o.TypeTitre);
        Assert.Equal("Emprunt 6,75 % 2026-2031", o.Nom);
        Assert.Equal(EmetteurB, o.EmetteurId);
        Assert.Equal(StatutTitre.Suspendu, o.Statut);
        Assert.Equal(6.75m, o.TauxFacial);
        Assert.Equal(new DateOnly(2026, 12, 30), o.DatePremierCoupon);
        Assert.Equal(2, o.DiffereEcheances);
        Assert.Equal("EXACT_360", o.CodeConvention);
        Assert.Equal(RegimeFiscal.Brut, o.RegimeFiscal);
    }

    [Fact]
    public void Une_modification_refusee_ne_change_rien()
    {
        var o = Obligation();

        // Nom et taux valables, mais échéance avant la jouissance : tout est refusé, rien n'est appliqué.
        var e = Assert.Throws<DomainException>(() => o.Modifier("Nouveau nom", EmetteurB, 20_000m, null, StatutTitre.Actif,
            9m, Periodicite.Annuelle, new DateOnly(2026, 6, 30), null, new DateOnly(2025, 6, 30),
            ModeRemboursement.InFine, 0, "EXACT_365", RegimeFiscal.Net));

        Assert.Contains("échéance", e.Message);
        Assert.Equal("Emprunt 6,50 % 2026-2031", o.Nom);
        Assert.Equal(EmetteurA, o.EmetteurId);
        Assert.Equal(10_000m, o.Nominal);
        Assert.Equal(6.5m, o.TauxFacial);
    }

    [Fact]
    public void Une_regle_commune_refusee_ne_change_pas_non_plus_les_champs_propres()
    {
        var a = Action();

        Assert.Throws<DomainException>(() => a.Modifier("  ", EmetteurA, 10_000m, null, StatutTitre.Actif, 2_000_000));

        Assert.Equal("Société Exemple SA", a.Nom);
        Assert.Equal(1_000_000, a.NombreTitres);
    }

    [Fact]
    public void Une_action_modifiee_prend_les_nouvelles_valeurs()
    {
        var a = Action();

        a.Modifier("Société Exemple", EmetteurB, 5_000m, new DateOnly(2010, 1, 4), StatutTitre.Actif, 2_000_000);

        Assert.Equal(("Société Exemple", EmetteurB, 5_000m, 2_000_000L), (a.Nom, a.EmetteurId, a.Nominal, a.NombreTitres));
        Assert.Equal(new DateOnly(2010, 1, 4), a.DateAdmission);
    }

    [Fact]
    public void Un_statut_inconnu_est_refuse()
    {
        var a = Action();
        Assert.Throws<DomainException>(() => a.Modifier("Société", EmetteurA, 10_000m, null, (StatutTitre)42, 1_000_000));
        Assert.Throws<DomainException>(() => a.ChangerStatut((StatutTitre)42));
        Assert.Equal(StatutTitre.Actif, a.Statut);
    }

    [Fact]
    public void Radier_retire_le_titre_de_la_cote()
    {
        var o = Obligation();
        o.Radier();
        Assert.Equal(StatutTitre.Radie, o.Statut);
    }
}
