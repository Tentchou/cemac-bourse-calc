using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>
/// Code ISIN (ISO 6166) : 2 lettres de pays, 9 caractères alphanumériques, 1 chiffre
/// de contrôle (algorithme de Luhn). Seuls les pays de la CEMAC sont acceptés.
/// </summary>
public sealed record Isin
{
    public string Valeur { get; }
    public PaysCemac Pays { get; }

    private Isin(string valeur, PaysCemac pays) { Valeur = valeur; Pays = pays; }

    public static Isin Creer(string? saisie)
    {
        var v = (saisie ?? string.Empty).Trim().ToUpperInvariant();

        if (v.Length != 12 || !v.All(char.IsAsciiLetterOrDigit) || !char.IsAsciiLetter(v[0]) || !char.IsAsciiLetter(v[1]))
            throw new DomainException($"« {saisie} » n'est pas un code ISIN : il faut 12 caractères, dont 2 lettres de pays au début.");

        if (!Enum.TryParse<PaysCemac>(v[..2], out var pays))
            throw new DomainException($"Le code ISIN {v} n'est pas celui d'un pays de la CEMAC (CM, CF, CG, GA, GQ, TD).");

        if (!char.IsAsciiDigit(v[11]) || ChiffreControle(v[..11]) != v[11] - '0')
            throw new DomainException($"Le chiffre de contrôle du code ISIN {v} est faux : vérifiez la saisie.");

        return new Isin(v, pays);
    }

    /// <summary>Chiffre de contrôle des 11 premiers caractères (lettres A=10 … Z=35, puis Luhn).</summary>
    public static int ChiffreControle(string onzeCaracteres)
    {
        var chiffres = string.Concat(onzeCaracteres.ToUpperInvariant()
            .Select(c => char.IsAsciiDigit(c) ? (c - '0').ToString() : (c - 'A' + 10).ToString()));

        var somme = 0;
        var doubler = true; // le chiffre le plus à droite est doublé (le contrôle viendra après lui)
        for (var i = chiffres.Length - 1; i >= 0; i--)
        {
            var d = chiffres[i] - '0';
            if (doubler) { d *= 2; if (d > 9) d -= 9; }
            somme += d;
            doubler = !doubler;
        }
        return (10 - somme % 10) % 10;
    }

    public override string ToString() => Valeur;
}
