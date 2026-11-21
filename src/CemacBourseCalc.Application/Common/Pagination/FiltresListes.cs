namespace CemacBourseCalc.Application.Common.Pagination;

/// <summary>
/// Contrôle d'un filtre reçu en texte : seul le nom exact d'une valeur de l'énumération est accepté
/// (« CM », pas « cm » ni « 0 »). Le filtre peut ensuite aller en base comme paramètre, sans risque.
/// </summary>
public static class FiltresListes
{
    public static bool EstValeur<TEnum>(string? texte) where TEnum : struct, Enum =>
        texte is not null && Enum.GetNames<TEnum>().Contains(texte, StringComparer.Ordinal);
}
