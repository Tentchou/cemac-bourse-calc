using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Place;

namespace CemacBourseCalc.Domain.Tests.Place;

/// <summary>Les jours fériés certains du Cameroun, comparés aux publications de la BVMAC.</summary>
public class JoursFeriesCamerounTests
{
    private static DateOnly D(int a, int m, int j) => new(a, m, j);

    [Theory]
    [InlineData(2024, 3, 31)]
    [InlineData(2025, 4, 20)]
    [InlineData(2026, 4, 5)]
    [InlineData(2027, 3, 28)]
    [InlineData(2038, 4, 25)] // la date la plus tardive possible
    [InlineData(2285, 3, 22)] // la plus précoce possible (hors bornes de Calculer, mais Pâques reste juste)
    public void Paques_suit_le_calendrier_gregorien(int annee, int mois, int jour) =>
        Assert.Equal(D(annee, mois, jour), JoursFeriesCameroun.Paques(annee));

    [Fact]
    public void L_annee_2025_redonne_la_liste_publiee_par_la_BVMAC_sans_les_fetes_musulmanes()
    {
        // Page « Informations pratiques » de la BVMAC, liste 2025 (hors Aïd el-Fitr et Aïd el-Kebir, fixés à la lune).
        Assert.Equal(
            [D(2025, 1, 1), D(2025, 2, 11), D(2025, 4, 18), D(2025, 5, 1), D(2025, 5, 20), D(2025, 5, 29), D(2025, 8, 15), D(2025, 12, 25)],
            JoursFeriesCameroun.Calculer(2025).Select(j => j.Date));
    }

    [Fact]
    public void L_annee_2026_contient_chaque_fermeture_annoncee_par_avis()
    {
        var dates = JoursFeriesCameroun.Calculer(2026).Select(j => j.Date).ToHashSet();

        // Avis n° 005, 014, 021, 026 et 030/2026/BVMAC/DG.
        Assert.Subset(dates, new HashSet<DateOnly> { D(2026, 2, 11), D(2026, 4, 3), D(2026, 5, 1), D(2026, 5, 14), D(2026, 5, 20) });
        Assert.DoesNotContain(D(2026, 8, 15), dates); // un samedi : pas de séance de toute façon
    }

    [Fact]
    public void Une_fete_fixe_un_dimanche_est_chomee_le_lundi()
    {
        // Le dimanche 11/02/2024 : la BVMAC a fermé le lundi 12/02/2024.
        var jour = Assert.Single(JoursFeriesCameroun.Calculer(2024), j => j.Date.Month == 2);
        Assert.Equal(D(2024, 2, 12), jour.Date);
        Assert.Contains("reportée du dimanche 11/02/2024", jour.Libelle);
    }

    [Fact]
    public void Aucun_jour_rendu_ne_tombe_un_week_end_et_chaque_date_est_unique()
    {
        for (var annee = JoursFeriesCameroun.AnneeMinimum; annee <= JoursFeriesCameroun.AnneeMaximum; annee++)
        {
            var jours = JoursFeriesCameroun.Calculer(annee);
            Assert.All(jours, j => Assert.True(j.Date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday), $"{j.Date} {j.Libelle}"));
            Assert.Equal(jours.Count, jours.Select(j => j.Date).Distinct().Count());
            Assert.All(jours, j => Assert.Equal(annee, j.Date.Year));
        }
    }

    [Theory]
    [InlineData(1999)]
    [InlineData(2101)]
    public void Une_annee_hors_bornes_est_refusee(int annee) =>
        Assert.Throws<DomainException>(() => JoursFeriesCameroun.Calculer(annee));
}
