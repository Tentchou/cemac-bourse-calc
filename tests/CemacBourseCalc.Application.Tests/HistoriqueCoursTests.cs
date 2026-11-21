using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Application.Titres.Queries.ObtenirHistoriqueCours;

namespace CemacBourseCalc.Application.Tests;

public class HistoriqueCoursTests
{
    private sealed class LectureFixe(IReadOnlyList<PointCoursDto>? points) : IHistoriqueCoursLecture
    {
        public int? MaximumDemande { get; private set; }

        public Task<IReadOnlyList<PointCoursDto>?> HistoriqueAsync(Guid titreId, int maximum, CancellationToken ct)
        {
            MaximumDemande = maximum;
            return Task.FromResult(points);
        }
    }

    [Fact]
    public async Task L_historique_est_borne_a_environ_cinq_ans_de_seances()
    {
        var lecture = new LectureFixe([new PointCoursDto(new DateOnly(2026, 11, 20), 100m, 10)]);

        var points = await new ObtenirHistoriqueCoursQueryHandler(lecture).Handle(new ObtenirHistoriqueCoursQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(ObtenirHistoriqueCoursQuery.SeancesMaximum, lecture.MaximumDemande);
        Assert.Single(points!);
    }

    [Fact]
    public async Task Un_titre_inconnu_rend_null_et_un_titre_sans_cours_une_liste_vide()
    {
        Assert.Null(await new ObtenirHistoriqueCoursQueryHandler(new LectureFixe(null)).Handle(new ObtenirHistoriqueCoursQuery(Guid.NewGuid()), CancellationToken.None));
        Assert.Empty((await new ObtenirHistoriqueCoursQueryHandler(new LectureFixe([])).Handle(new ObtenirHistoriqueCoursQuery(Guid.NewGuid()), CancellationToken.None))!);
    }
}
