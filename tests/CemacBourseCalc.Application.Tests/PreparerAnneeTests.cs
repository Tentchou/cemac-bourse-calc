using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Place.Dtos;
using CemacBourseCalc.Application.Place.Interfaces;
using CemacBourseCalc.Application.Place.Queries.ListerJoursNonOuvres;
using CemacBourseCalc.Application.Place.Queries.PreparerAnnee;

namespace CemacBourseCalc.Application.Tests;

/// <summary>L'aperçu d'une année : le calcul croisé avec ce qui est déjà saisi.</summary>
public class PreparerAnneeTests
{
    private sealed class CalendrierFixe(params JourNonOuvreDto[] jours) : IJoursNonOuvresLecture
    {
        public Task<PageDto<JourNonOuvreDto>> PageAsync(ListerJoursNonOuvresQuery q, CancellationToken ct) => throw new NotSupportedException();

        public Task<IReadOnlyList<JourNonOuvreDto>> DeLAnneeAsync(int annee, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<JourNonOuvreDto>>(jours.Where(j => j.Date.Year == annee).ToList());
    }

    private static DateOnly D(int a, int m, int j) => new(a, m, j);

    [Fact]
    public async Task Les_jours_deja_saisis_sont_signales_et_les_autres_saisies_de_l_annee_sont_rendues()
    {
        var calendrier = new CalendrierFixe(
            new JourNonOuvreDto(D(2026, 1, 1), "Nouvel an"),
            new JourNonOuvreDto(D(2026, 3, 20), "Aïd el-Fitr"),     // fête musulmane, jamais calculée
            new JourNonOuvreDto(D(2025, 12, 25), "Noël"));           // autre année : ignorée

        var apercu = await new PreparerAnneeQueryHandler(calendrier).Handle(new PreparerAnneeQuery(2026), CancellationToken.None);

        var nouvelAn = Assert.Single(apercu.Proposes, j => j.Date == D(2026, 1, 1));
        Assert.True(nouvelAn.DejaEnregistre);
        Assert.Equal("Nouvel an", nouvelAn.LibelleEnregistre);
        Assert.All(apercu.Proposes.Where(j => j.Date != D(2026, 1, 1)), j => Assert.False(j.DejaEnregistre));
        Assert.Equal([D(2026, 3, 20)], apercu.AutresEnregistres.Select(j => j.Date));
    }

    [Theory]
    [InlineData(1999, false)]
    [InlineData(2000, true)]
    [InlineData(2100, true)]
    [InlineData(2101, false)]
    public void L_annee_est_bornee(int annee, bool valide) =>
        Assert.Equal(valide, new PreparerAnneeQueryValidator().Validate(new PreparerAnneeQuery(annee)).IsValid);
}
