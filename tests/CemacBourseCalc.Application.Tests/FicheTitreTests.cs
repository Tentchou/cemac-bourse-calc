using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Application.Titres.Queries.ObtenirFicheTitre;

namespace CemacBourseCalc.Application.Tests;

public class FicheTitreTests
{
    private sealed class LectureFixe(FicheTitreDto? fiche) : ITitresLecture
    {
        public Task<IReadOnlyList<TitreResumeDto>> ListerAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<TitreResumeDto>>([]);
        public Task<FicheTitreDto?> FicheAsync(Guid id, CancellationToken ct) => Task.FromResult(fiche);
    }

    private static FicheTitreDto Obligation(string codeConvention) => new(
        Guid.NewGuid(), "CM0000000011", "Exemple 6,50 %", "Émetteur A", "CM", "Obligation", 10_000m, "Actif",
        98.5m, new DateOnly(2026, 1, 12),
        new FicheObligationDto(6.5m, "Annuelle", "InFine", new DateOnly(2025, 6, 30), null, new DateOnly(2030, 6, 30), codeConvention, "Net"),
        null);

    [Theory]
    [InlineData("EXACT_365", "Exact/365")]
    [InlineData("EXACT_360", "Exact/360")]
    [InlineData("30E_360", "30E/360")]
    public async Task La_convention_est_rendue_par_son_libelle(string code, string libelle)
    {
        var handler = new ObtenirFicheTitreQueryHandler(new LectureFixe(Obligation(code)));

        var fiche = await handler.Handle(new ObtenirFicheTitreQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(libelle, fiche!.Obligation!.Convention);
        Assert.Equal(6.5m, fiche.Obligation.TauxFacial);
    }

    [Fact]
    public async Task Un_titre_inconnu_rend_null()
    {
        var handler = new ObtenirFicheTitreQueryHandler(new LectureFixe(null));

        Assert.Null(await handler.Handle(new ObtenirFicheTitreQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Une_action_passe_sans_changement()
    {
        var action = new FicheTitreDto(Guid.NewGuid(), "CM0000000045", "Société Exemple SA", "Société Exemple", "CM", "Action",
            10_000m, "Actif", 32_000m, new DateOnly(2026, 1, 12), null,
            new FicheActionDto(1_000_000, [new DividendeDto(2025, 1_500m, 1_260m, new DateOnly(2026, 6, 15), new DateOnly(2026, 6, 30))]));
        var handler = new ObtenirFicheTitreQueryHandler(new LectureFixe(action));

        Assert.Same(action, await handler.Handle(new ObtenirFicheTitreQuery(action.Id), CancellationToken.None));
    }
}
