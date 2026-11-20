using System.Security.Cryptography;
using System.Text;

namespace CemacBourseCalc.Application.Administration.Common;

/// <summary>
/// Jetons de session : 32 octets aléatoires (générateur cryptographique). On stocke leur empreinte
/// SHA-256 : une fuite de la table ne permet pas d'ouvrir une session.
/// </summary>
public static class JetonsSession
{
    public static readonly TimeSpan Duree = TimeSpan.FromHours(8);

    public static (string Jeton, string Empreinte) Generer()
    {
        var jeton = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return (jeton, Empreinte(jeton));
    }

    public static string Empreinte(string jeton) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(jeton)));

    /// <summary>Un jeton émis fait 43 caractères ; tout autre format est refusé sans requête en base.</summary>
    public static bool FormatValide(string? jeton) =>
        jeton is { Length: 43 } && jeton.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_');
}
