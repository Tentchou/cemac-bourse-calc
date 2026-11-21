using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CemacBourseCalc.Application.Bulletins.Dtos;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Application.Bulletins.Common;

/// <summary>
/// Lit les cours de la séance dans le bulletin officiel de la cote de la BVMAC, à partir des mots du PDF
/// et de leur position. Fonction pure, testée sur de vrais bulletins.
///
/// Pages lues : celles du tableau de la séance (« Séance de cotation » et « Statut »). Sur chaque ligne
/// portant un code ISIN, les colonnes sont repérées à partir de deux ancres sûres : la date du cours
/// précédent, puis le statut de cotation (NC, PEq…), premier mot en lettres après cette date :
///  - obligation : date, cours %, nominal, nominal J+3, coupon couru, 3 volumes, valeur, nombre, STATUT, ouverture, clôture…
///  - action     : date, 3 volumes, valeur, nombre, STATUT, ouverture, clôture…
/// Si le nombre de colonnes entre les ancres n'est pas celui attendu, la ligne est déclarée illisible.
/// </summary>
public static partial class AnalyseurBulletin
{
    public const string SectionObligation = nameof(TypeTitre.Obligation);
    public const string SectionAction = nameof(TypeTitre.Action);

    /// <summary>Écart maximal (points) entre « 6 » et « 000,00 » d'un même nombre ; entre deux colonnes, il dépasse 3,3 points.</summary>
    private const double EcartMilliers = 2.8;

    // Colonnes attendues entre la date (incluse) et le statut (exclu).
    private const int ColonnesObligation = 10;
    private const int ColonnesAction = 6;

    [GeneratedRegex(@"(CM|CF|CG|GA|GQ|TD)\d{10}$")]
    private static partial Regex FinIsin();

    [GeneratedRegex(@"^(CM|CF|CG|GA|GQ|TD)$")]
    private static partial Regex CodePays();

    [GeneratedRegex(@"^\d{10}$")]
    private static partial Regex DixChiffres();

    [GeneratedRegex(@"^\d{2}/\d{2}/\d{4}$")]
    private static partial Regex Date();

    [GeneratedRegex(@"^\p{L}+$")]
    private static partial Regex Statut();

    [GeneratedRegex(@"^\d{1,3}$")]
    private static partial Regex DebutMilliers();

    [GeneratedRegex(@"^\d{3}([,.]\d+)?$")]
    private static partial Regex GroupeMilliers();

    [GeneratedRegex(@"BULLETIN OFFICIEL DE LA COTE N\W{0,3}\s*(\d{1,6})\s+DU\s+(\d{2}/\d{2}/\d{4})")]
    private static partial Regex EnTete();

    /// <summary>Une page du tableau de la séance (actions ou obligations).</summary>
    public static bool EstPageDeSeance(PageBulletin page)
    {
        var texte = TexteNormalise(page);
        return texte.Contains("SEANCE DE COTATION") && texte.Contains("STATUT");
    }

    /// <summary>
    /// Vrai dès que les tableaux de la séance sont passés (une page qui n'en est pas, après au moins une qui en est) :
    /// le reste du bulletin (capitalisation, fiches des émetteurs) n'a pas besoin d'être lu.
    /// </summary>
    public static bool SeanceLue(IReadOnlyList<PageBulletin> pagesLues) =>
        pagesLues.Count >= 2 && !EstPageDeSeance(pagesLues[^1]) && pagesLues.Take(pagesLues.Count - 1).Any(EstPageDeSeance);

    public static Result<BulletinLu> Analyser(IReadOnlyList<PageBulletin> pages)
    {
        var entete = pages.Select(p => EnTete().Match(TexteNormalise(p))).FirstOrDefault(m => m.Success);
        if (entete is null
            || !int.TryParse(entete.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var numero)
            || !DateOnly.TryParseExact(entete.Groups[2].Value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateSeance))
            return Result<BulletinLu>.Echec("Ce fichier n'est pas un bulletin officiel de la cote de la BVMAC (en-tête introuvable).");

        var cours = new List<CoursLu>();
        var anomalies = new List<string>();

        foreach (var page in pages)
        {
            if (!EstPageDeSeance(page)) continue;
            var texte = TexteNormalise(page);
            var section = texte.Contains("MARCHE DES ACTIONS") ? SectionAction : texte.Contains("OBLIGATIONS") ? SectionObligation : null;
            if (section is null) continue;

            foreach (var ligne in Lignes(page.Mots))
            {
                var mots = Isoler(Fusionner(ligne));
                var position = mots.FindIndex(m => m.Texte.Length == 12 && FinIsin().IsMatch(m.Texte));
                if (position < 0) continue;
                var isin = mots[position].Texte;
                var libelle = string.Join(' ', mots.Take(position).Select(m => m.Texte));

                var lu = LireLigne(mots, position, section);
                if (lu is null)
                {
                    anomalies.Add($"Page {page.Numero}, {isin} : colonnes illisibles.");
                    continue;
                }
                cours.Add(new CoursLu(page.Numero, isin, libelle, section, lu.Value.Cloture, lu.Value.Volume, lu.Value.Coupon));
            }
        }

        foreach (var doublon in cours.GroupBy(c => c.Isin).Where(g => g.Count() > 1))
            anomalies.Add($"{doublon.Key} figure {doublon.Count()} fois dans les tableaux de la séance.");

        if (cours.Count == 0 && anomalies.Count == 0)
            return Result<BulletinLu>.Echec("Aucun cours de séance n'a été trouvé dans ce bulletin.");

        return Result<BulletinLu>.Succes(new BulletinLu(numero, dateSeance, cours, anomalies));
    }

    private static (decimal? Cloture, long? Volume, decimal? Coupon)? LireLigne(List<MotPdf> mots, int isin, string section)
    {
        var date = mots.FindIndex(isin + 1, m => Date().IsMatch(m.Texte));
        if (date < 0) return null;
        var statut = mots.FindIndex(date + 1, m => Statut().IsMatch(m.Texte));
        if (statut < 0 || statut + 2 >= mots.Count) return null;

        var attendu = section == SectionObligation ? ColonnesObligation : ColonnesAction;
        if (statut - date != attendu) return null;

        var cloture = Nombre(mots[statut + 2].Texte);
        var volume = Entier(mots[statut - 3].Texte);
        decimal? coupon = null;
        if (section == SectionObligation)
        {
            coupon = Nombre(mots[date + 4].Texte);
            if (coupon is null) return null;
        }
        if (cloture is null || volume is null) return null;
        return (cloture, volume, coupon);
    }

    /// <summary>Les mots d'une page regroupés en lignes (même ligne de base, à une demi-hauteur près), de gauche à droite.</summary>
    private static IEnumerable<List<MotPdf>> Lignes(IReadOnlyList<MotPdf> mots)
    {
        var lignes = new List<(double Y, List<MotPdf> Mots)>();
        foreach (var mot in mots.OrderBy(m => m.Y).ThenBy(m => m.X))
        {
            var tolerance = Math.Max(1.5, mot.Hauteur * 0.5);
            var ligne = lignes.Count > 0 && Math.Abs(lignes[^1].Y - mot.Y) <= tolerance ? lignes[^1] : default;
            if (ligne.Mots is null)
                lignes.Add((mot.Y, [mot]));
            else
                ligne.Mots.Add(mot);
        }
        return lignes.Select(l => l.Mots.OrderBy(m => m.X).ToList());
    }

    /// <summary>« 6 » + « 000,00 » très proches → « 6000,00 » (séparateur de milliers imprimé comme une espace).</summary>
    private static List<MotPdf> Fusionner(List<MotPdf> mots)
    {
        var sortie = new List<MotPdf>(mots.Count);
        foreach (var mot in mots)
        {
            if (sortie.Count > 0)
            {
                var precedent = sortie[^1];
                var chiffres = DebutMilliers().IsMatch(precedent.Texte) || (precedent.Texte.Length > 3 && precedent.Texte.All(char.IsAsciiDigit));
                if (chiffres && GroupeMilliers().IsMatch(mot.Texte) && mot.X - precedent.X2 < EcartMilliers)
                {
                    sortie[^1] = precedent with { X2 = mot.X2, Texte = precedent.Texte + mot.Texte };
                    continue;
                }
            }
            sortie.Add(mot);
        }
        return sortie;
    }

    /// <summary>
    /// Sépare un ISIN collé au nom du titre (« 2024-2027GA0000020511 ») et recolle un ISIN coupé
    /// après le code pays (« CM » « 0000010009 »), pour qu'il soit un mot à lui seul.
    /// </summary>
    private static List<MotPdf> Isoler(List<MotPdf> mots)
    {
        var sortie = new List<MotPdf>(mots.Count + 1);
        for (var i = 0; i < mots.Count; i++)
        {
            var mot = mots[i];
            if (CodePays().IsMatch(mot.Texte) && i + 1 < mots.Count && DixChiffres().IsMatch(mots[i + 1].Texte))
            {
                sortie.Add(mot with { X2 = mots[i + 1].X2, Texte = mot.Texte + mots[i + 1].Texte });
                i++;
                continue;
            }
            var fin = FinIsin().Match(mot.Texte);
            if (fin.Success && fin.Index > 0)
            {
                sortie.Add(mot with { Texte = mot.Texte[..fin.Index] });
                sortie.Add(mot with { Texte = fin.Value });
                continue;
            }
            sortie.Add(mot);
        }
        return sortie;
    }

    private static decimal? Nombre(string texte) =>
        decimal.TryParse(texte.Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var n) ? n : null;

    private static long? Entier(string texte) =>
        long.TryParse(texte, NumberStyles.None, CultureInfo.InvariantCulture, out var n) ? n : null;

    /// <summary>Le texte de la page en majuscules, sans accents ni espaces multiples (pour repérer en-tête et sections).</summary>
    private static string TexteNormalise(PageBulletin page)
    {
        var texte = string.Join(' ', page.Mots.Select(m => m.Texte)).Normalize(NormalizationForm.FormD);
        var sansAccents = new StringBuilder(texte.Length);
        foreach (var c in texte)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sansAccents.Append(c);
        return Regex.Replace(sansAccents.ToString().ToUpperInvariant(), @"\s+", " ");
    }
}
