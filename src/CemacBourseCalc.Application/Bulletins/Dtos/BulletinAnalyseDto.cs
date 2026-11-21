using CemacBourseCalc.Application.Titres.Dtos;

namespace CemacBourseCalc.Application.Bulletins.Dtos;

/// <summary>
/// Une ligne du bulletin rapprochée de la base. TitreId null : titre inconnu. Probleme : la raison qui
/// empêche d'enregistrer cette ligne (titre inconnu, type différent). Controle : coupon couru publié face
/// à notre calcul (obligations).
/// </summary>
public sealed record LigneBulletinDto(
    string Isin,
    string Nom,
    string TypeTitre,
    Guid? TitreId,
    decimal? CoursCloture,
    long? Volume,
    decimal? CouponCouruPublie,
    ControleCouponCouruDto? Controle,
    string? Probleme);

/// <summary>Un titre de la base absent des tableaux de la séance (information, ne bloque pas).</summary>
public sealed record TitreAbsentDto(string Isin, string Nom, string TypeTitre);

/// <summary>
/// L'aperçu d'un bulletin, avant tout enregistrement. Importable seulement si aucune ligne n'est
/// illisible ou inconnue et si la séance n'a pas déjà été saisie.
/// </summary>
public sealed record BulletinAnalyseDto(
    int Numero,
    DateOnly DateSeance,
    int CoursDejaEnregistres,
    IReadOnlyList<LigneBulletinDto> Lignes,
    IReadOnlyList<string> Anomalies,
    IReadOnlyList<TitreAbsentDto> Absents,
    bool Importable);
