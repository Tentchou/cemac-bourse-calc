using CemacBourseCalc.Domain.Place;

namespace CemacBourseCalc.Domain.Tests.Place;

public class JoursOuvresTests
{
    private static readonly IReadOnlySet<DateOnly> Aucun = new HashSet<DateOnly>();

    [Fact]
    public void T_plus_3_saute_le_week_end()
    {
        // Négociée le vendredi 16/10/2026 : lundi T+1, mardi T+2, mercredi 21/10 T+3.
        Assert.Equal(new DateOnly(2026, 10, 21), JoursOuvres.DateReglement(new DateOnly(2026, 10, 16), 3, Aucun));
    }

    [Fact]
    public void T_plus_3_saute_aussi_un_jour_non_ouvre()
    {
        // Lundi 19/10/2026 non ouvré : mardi T+1, mercredi T+2, jeudi 22/10 T+3.
        var nonOuvres = new HashSet<DateOnly> { new(2026, 10, 19) };
        Assert.Equal(new DateOnly(2026, 10, 22), JoursOuvres.DateReglement(new DateOnly(2026, 10, 16), 3, nonOuvres));
    }

    [Fact]
    public void T_plus_0_est_le_jour_meme()
    {
        Assert.Equal(new DateOnly(2026, 10, 16), JoursOuvres.DateReglement(new DateOnly(2026, 10, 16), 0, Aucun));
    }
}
