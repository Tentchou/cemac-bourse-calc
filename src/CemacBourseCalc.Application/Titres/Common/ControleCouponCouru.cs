using CemacBourseCalc.Application.Calculs.Common;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Place;

namespace CemacBourseCalc.Application.Titres.Common;

/// <summary>Une obligation à contrôler : ses caractéristiques, la séance et le coupon couru publié par la BVMAC.</summary>
public sealed record ObligationAControler(CaracteristiquesObligation Caracteristiques, DateOnly DateSeance, decimal Publie);

/// <summary>
/// Compare notre coupon couru (par titre) à celui publié par la BVMAC, pour les lignes d'une page.
/// Même calcul que la calculatrice publique : règlement à J+délai jours ouvrés, jours non ouvrés compris.
/// Deux lectures seulement pour toute la page (le délai, puis le calendrier de la période couverte).
/// </summary>
public static class ControleCouponCouru
{
    /// <summary>Borne du calendrier lu après la dernière séance : un délai de règlement ne dépasse jamais quelques semaines.</summary>
    private const int FenetreCalendrierJours = 60;

    /// <summary>Un résultat par ligne, dans le même ordre ; null quand la ligne n'a rien à contrôler.</summary>
    public static async Task<IReadOnlyList<ControleCouponCouruDto?>> ControlerAsync(
        ILectureCalculs lecture, IReadOnlyList<ObligationAControler?> lignes, CancellationToken ct)
    {
        var aControler = lignes.OfType<ObligationAControler>().ToList();
        if (aControler.Count == 0) return lignes.Select(_ => (ControleCouponCouruDto?)null).ToList();

        var delai = await ReglesPlace.DelaiReglementAsync(lecture, ct);
        if (delai is null)
            return lignes.Select(l => l is null ? null : Echec(l, null, ReglesPlace.MessageDelaiAbsent)).ToList();

        var nonOuvres = await lecture.JoursNonOuvresAsync(
            aControler.Min(l => l.DateSeance), aControler.Max(l => l.DateSeance).AddDays(FenetreCalendrierJours), ct);

        return lignes.Select(l => l is null ? null : Controler(l, delai.Value, nonOuvres)).ToList();
    }

    private static ControleCouponCouruDto Controler(ObligationAControler l, int delai, IReadOnlySet<DateOnly> nonOuvres)
    {
        var reglement = JoursOuvres.DateReglement(l.DateSeance, delai, nonOuvres);
        try
        {
            var calcule = CouponCouru.Calculer(l.Caracteristiques, reglement, 1).ParTitre;
            var ecart = calcule - l.Publie;
            return new ControleCouponCouruDto(l.DateSeance, l.Publie, reglement, calcule, ecart, ecart == 0m, null);
        }
        catch (DomainException e)
        {
            return Echec(l, reglement, e.Message);
        }
    }

    private static ControleCouponCouruDto Echec(ObligationAControler l, DateOnly? reglement, string message) =>
        new(l.DateSeance, l.Publie, reglement, null, null, false, message);
}
