namespace CemacBourseCalc.Application.Common.Pagination;

/// <summary>
/// Les bornes de la pagination. Elles protègent la base : une page ne dépasse jamais 50 lignes,
/// le décalage reste borné, et la recherche est un motif LIKE dont les jokers saisis sont neutralisés.
/// </summary>
public static class Pagination
{
    /// <summary>Les tailles de page acceptées (les mêmes que l'écran).</summary>
    public static readonly IReadOnlySet<int> Tailles = new HashSet<int> { 10, 20, 50 };

    public const int PageMaximum = 10_000;
    public const int RechercheMaximum = 100;

    /// <summary>Nombre de lignes à sauter pour atteindre la page.</summary>
    public static int Decalage(int page, int taille) => (page - 1) * taille;

    /// <summary>
    /// Le motif ILIKE de la recherche (« %texte% »), ou null si rien n'est cherché. Les caractères
    /// \ % _ saisis sont échappés : ils sont cherchés tels quels (la requête précise ESCAPE '\').
    /// </summary>
    public static string? Motif(string? recherche)
    {
        var texte = recherche?.Trim();
        if (string.IsNullOrEmpty(texte)) return null;
        var echappe = texte.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
        return $"%{echappe}%";
    }
}
