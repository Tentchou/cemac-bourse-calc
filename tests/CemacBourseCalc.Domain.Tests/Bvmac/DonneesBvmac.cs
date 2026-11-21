using System.Globalization;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;

namespace CemacBourseCalc.Domain.Tests.Bvmac;

/// <summary>
/// Lit les fichiers d'import tirés d'un bulletin officiel de la cote BVMAC (dossier donnees/bvmac).
/// Ce sont les mêmes fichiers que l'écran d'import : ce que l'on compare est ce que l'on importe.
/// </summary>
public static class DonneesBvmac
{
    /// <summary>Le bulletin n° 2643 du 20/11/2026.</summary>
    public const string Bulletin2643 = "2026-11-20_BOC-2643";

    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr-FR");

    public static string Dossier(string bulletin)
    {
        var dossier = new DirectoryInfo(AppContext.BaseDirectory);
        while (dossier is not null && !File.Exists(Path.Combine(dossier.FullName, "CemacBourseCalc.slnx")))
            dossier = dossier.Parent;
        if (dossier is null) throw new InvalidOperationException("Racine du dépôt introuvable (CemacBourseCalc.slnx).");
        return Path.Combine(dossier.FullName, "donnees", "bvmac", bulletin);
    }

    /// <summary>Les lignes d'un fichier « ; », chacune indexée par son en-tête.</summary>
    public static IReadOnlyList<IReadOnlyDictionary<string, string>> Lire(string bulletin, string fichier)
    {
        var lignes = File.ReadAllLines(Path.Combine(Dossier(bulletin), fichier))
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Split(';'))
            .ToList();
        var entetes = lignes[0].Select(e => e.Trim().TrimStart('﻿')).ToArray();

        return lignes.Skip(1)
            .Select(cellules => (IReadOnlyDictionary<string, string>)entetes
                .Select((e, i) => (e, v: i < cellules.Length ? cellules[i].Trim() : ""))
                .ToDictionary(x => x.e, x => x.v))
            .ToList();
    }

    public static decimal Nombre(string s) => decimal.Parse(s.Replace(" ", ""), NumberStyles.Number, Fr);

    public static decimal? NombreOuRien(string s) => string.IsNullOrWhiteSpace(s) ? null : Nombre(s);

    public static DateOnly Date(string s) => DateOnly.ParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture);

    public static DateOnly? DateOuRien(string s) => string.IsNullOrWhiteSpace(s) ? null : Date(s);

    /// <summary>Une ligne du fichier des obligations, créée avec les règles du Domain (ISIN, dates, différé…).</summary>
    public static Obligation Obligation(IReadOnlyDictionary<string, string> l) => new(
        Isin.Creer(l["ISIN"]),
        l["Nom"],
        Guid.NewGuid(),
        Nombre(l["Nominal"]),
        DateOuRien(l["Date d'admission"]),
        Nombre(l["Taux facial"]),
        l["Périodicité"] switch
        {
            "Annuelle" => Periodicite.Annuelle,
            "Semestrielle" => Periodicite.Semestrielle,
            "Trimestrielle" => Periodicite.Trimestrielle,
            "Mensuelle" => Periodicite.Mensuelle,
            "In fine" => Periodicite.InFine,
            var p => throw new InvalidOperationException($"Périodicité inconnue : {p}")
        },
        Date(l["Date de jouissance"]),
        DateOuRien(l["Date du premier coupon"]),
        Date(l["Date d'échéance"]),
        l["Remboursement"] switch
        {
            "In fine" => ModeRemboursement.InFine,
            "Amortissement constant" => ModeRemboursement.AmortissementConstant,
            var m => throw new InvalidOperationException($"Remboursement inconnu : {m}")
        },
        string.IsNullOrWhiteSpace(l["Différé"]) ? 0 : int.Parse(l["Différé"], CultureInfo.InvariantCulture),
        l["Convention"] switch
        {
            "Exact/365" => "EXACT_365",
            "Exact/360" => "EXACT_360",
            "30E/360" => "30E_360",
            var c => c
        },
        Enum.Parse<RegimeFiscal>(l["Régime fiscal"]));
}
