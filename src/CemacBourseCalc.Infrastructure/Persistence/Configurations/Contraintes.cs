using CemacBourseCalc.Domain.Conventions;

namespace CemacBourseCalc.Infrastructure.Persistence.Configurations;

/// <summary>
/// Les contraintes CHECK sont générées depuis le code (enums, conventions) : la liste des valeurs
/// permises n'est écrite qu'à un seul endroit. Ajouter une valeur demande une nouvelle migration.
/// </summary>
internal static class Contraintes
{
    public static string ValeursEnum<TEnum>(string colonne) where TEnum : struct, Enum =>
        Dans(colonne, Enum.GetNames<TEnum>());

    public static string Conventions(string colonne) =>
        Dans(colonne, ConventionsDecompte.Toutes.Select(c => c.Code));

    private static string Dans(string colonne, IEnumerable<string> valeurs) =>
        $"{colonne} IN ({string.Join(", ", valeurs.Select(v => $"'{v}'"))})";
}
