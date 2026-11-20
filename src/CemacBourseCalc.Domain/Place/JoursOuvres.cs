namespace CemacBourseCalc.Domain.Place;

/// <summary>
/// Les jours de séance de la BVMAC : ni samedi, ni dimanche, ni jour non ouvré du calendrier.
/// Calcul pur : le calendrier arrive en paramètre.
/// </summary>
public static class JoursOuvres
{
    public static bool EstOuvre(DateOnly jour, IReadOnlySet<DateOnly> nonOuvres) =>
        jour.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday) && !nonOuvres.Contains(jour);

    /// <summary>T + N jours ouvrés. N = 0 : le jour même.</summary>
    public static DateOnly Ajouter(DateOnly t, int n, IReadOnlySet<DateOnly> nonOuvres)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        var jour = t;
        while (n > 0)
        {
            jour = jour.AddDays(1);
            if (EstOuvre(jour, nonOuvres)) n--;
        }
        return jour;
    }

    /// <summary>Un paiement prévu un jour non ouvré est reporté au premier jour ouvré qui suit.</summary>
    public static DateOnly ReporterAuJourOuvreSuivant(DateOnly jour, IReadOnlySet<DateOnly> nonOuvres)
    {
        var date = jour;
        while (!EstOuvre(date, nonOuvres)) date = date.AddDays(1);
        return date;
    }

    /// <summary>
    /// Date de règlement-livraison d'une négociation : T + délai jours ouvrés. Le coupon couru
    /// court jusqu'à cette date, comme au bulletin de la BVMAC.
    /// </summary>
    public static DateOnly DateReglement(DateOnly dateNegociation, int delaiJoursOuvres, IReadOnlySet<DateOnly> nonOuvres) =>
        Ajouter(dateNegociation, delaiJoursOuvres, nonOuvres);
}
