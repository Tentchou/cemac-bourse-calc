namespace CemacBourseCalc.Domain.Place;

/// <summary>Un jour férié calculé, proposé à l'administrateur avant d'être enregistré comme jour non ouvré.</summary>
public sealed record JourFerieCalcule(DateOnly Date, string Libelle);

/// <summary>
/// Les jours fériés certains du Cameroun, État siège de la BVMAC : la cote ferme ces jours-là
/// (avis n° 001/2024/BVMAC/DG : « à l'exception des jours déclarés fériés sur le territoire de la
/// République du Cameroun »). Liste vérifiée contre celle publiée par la BVMAC pour 2025 et contre
/// ses avis de fermeture de 2026.
///
/// Seuls les jours qui se calculent sans erreur sont rendus : les six dates fixes, et le Vendredi saint
/// et l'Ascension, qui suivent Pâques. Les deux fêtes musulmanes (Aïd el-Fitr, Aïd el-Kebir) dépendent
/// de l'observation de la lune et sont fixées par les autorités quelques jours avant : elles sont
/// saisies à la publication de l'avis de la BVMAC, jamais devinées.
///
/// Une fête fixe qui tombe un dimanche est chômée le lundi suivant (avis du 12/02/2024 : fête de la
/// jeunesse du dimanche 11/02/2024). Un jour qui tombe un samedi ou un dimanche n'est pas rendu : il
/// n'y a pas de séance ces jours-là de toute façon.
/// </summary>
public static class JoursFeriesCameroun
{
    public const int AnneeMinimum = 2000;
    public const int AnneeMaximum = 2100;

    private static readonly (int Mois, int Jour, string Libelle)[] Fixes =
    [
        (1, 1, "Jour de l'an"),
        (2, 11, "Fête de la jeunesse"),
        (5, 1, "Fête du travail"),
        (5, 20, "Fête nationale"),
        (8, 15, "Assomption"),
        (12, 25, "Noël"),
    ];

    /// <summary>Les jours fériés certains d'une année tombant un jour de semaine, dans l'ordre des dates.</summary>
    public static IReadOnlyList<JourFerieCalcule> Calculer(int annee)
    {
        if (annee is < AnneeMinimum or > AnneeMaximum)
            throw new Communs.DomainException($"L'année doit être comprise entre {AnneeMinimum} et {AnneeMaximum}.");

        var jours = new List<JourFerieCalcule>();
        foreach (var (mois, jour, libelle) in Fixes)
        {
            var date = new DateOnly(annee, mois, jour);
            if (date.DayOfWeek == DayOfWeek.Sunday)
                jours.Add(new JourFerieCalcule(date.AddDays(1), $"{libelle} (reportée du dimanche {date:dd/MM/yyyy})"));
            else if (date.DayOfWeek != DayOfWeek.Saturday)
                jours.Add(new JourFerieCalcule(date, libelle));
        }

        var paques = Paques(annee);
        jours.Add(new JourFerieCalcule(paques.AddDays(-2), "Vendredi saint"));
        jours.Add(new JourFerieCalcule(paques.AddDays(39), "Ascension"));

        // Un report du lundi peut tomber sur un autre férié : la date n'est gardée qu'une fois.
        return jours.GroupBy(j => j.Date).Select(g => g.First()).OrderBy(j => j.Date).ToList();
    }

    /// <summary>Le dimanche de Pâques du calendrier grégorien (algorithme de Meeus, Jones et Butcher).</summary>
    public static DateOnly Paques(int annee)
    {
        var a = annee % 19;
        var b = annee / 100;
        var c = annee % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var mois = (h + l - 7 * m + 114) / 31;
        var jour = (h + l - 7 * m + 114) % 31 + 1;
        return new DateOnly(annee, mois, jour);
    }
}
