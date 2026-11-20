using System.Globalization;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Domain.Place;

namespace CemacBourseCalc.Application.Calculs.Common;

/// <summary>
/// Lecture des paramètres de la place utiles aux calculs. Un paramètre absent ou illisible rend le
/// calcul indisponible avec un message clair, plutôt qu'un résultat faux.
/// </summary>
public static class ReglesPlace
{
    public const int DelaiMaximum = 15;
    public const string ReportJourOuvreSuivant = "JOUR_OUVRE_SUIVANT";

    public const string MessageDelaiAbsent =
        "Le délai de règlement de la place n'est pas paramétré : le calcul est indisponible pour le moment.";
    public const string MessageReportInconnu =
        "La règle de report des paiements de la place n'est pas prise en charge : l'échéancier est indisponible pour le moment.";

    /// <summary>Le délai T+N en jours ouvrés, ou null s'il est absent ou hors bornes.</summary>
    public static async Task<int?> DelaiReglementAsync(ILectureCalculs lecture, CancellationToken ct)
    {
        var valeur = await lecture.ParametreAsync(ClesParametres.DelaiReglement, ct);
        return int.TryParse(valeur, NumberStyles.None, CultureInfo.InvariantCulture, out var n) && n <= DelaiMaximum ? n : null;
    }

    /// <summary>Vrai si la règle de report paramétrée est celle que le calcul applique.</summary>
    public static async Task<bool> ReportPrisEnChargeAsync(ILectureCalculs lecture, CancellationToken ct) =>
        string.Equals(await lecture.ParametreAsync(ClesParametres.RegleReport, ct), ReportJourOuvreSuivant, StringComparison.Ordinal);
}
