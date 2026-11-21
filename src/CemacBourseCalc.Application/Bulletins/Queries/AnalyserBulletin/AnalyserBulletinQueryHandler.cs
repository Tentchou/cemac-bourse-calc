using CemacBourseCalc.Application.Bulletins.Common;
using CemacBourseCalc.Application.Bulletins.Dtos;
using CemacBourseCalc.Application.Bulletins.Interfaces;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Titres.Common;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Bulletins.Queries.AnalyserBulletin;

/// <summary>
/// PDF → mots → cours de la séance → rapprochement par ISIN avec la base → contrôle du coupon couru publié.
/// Les règles validées : un titre inconnu (ou de type différent) bloque tout l'import ; un écart de coupon
/// couru est signalé sans bloquer (la valeur publiée est enregistrée) ; une séance déjà saisie n'est pas réimportée.
/// </summary>
public sealed class AnalyserBulletinQueryHandler(ILecteurPdf pdf, IBulletinsLecture lecture, ILectureCalculs calculs)
    : IRequestHandler<AnalyserBulletinQuery, Result<BulletinAnalyseDto>>
{
    public async Task<Result<BulletinAnalyseDto>> Handle(AnalyserBulletinQuery q, CancellationToken cancellationToken)
    {
        var pages = pdf.Lire(q.Contenu, AnalyserBulletinQuery.PagesMaximum, AnalyseurBulletin.SeanceLue);
        if (!pages.Reussi) return Result<BulletinAnalyseDto>.Echec(pages.Message!);

        var lu = AnalyseurBulletin.Analyser(pages.Valeur!);
        if (!lu.Reussi) return Result<BulletinAnalyseDto>.Echec(lu.Message!);
        var bulletin = lu.Valeur!;

        var titres = (await lecture.TitresAsync(cancellationToken)).ToDictionary(t => t.Isin, StringComparer.Ordinal);
        var dejaEnregistres = await lecture.CoursDeLaSeanceAsync(bulletin.DateSeance, cancellationToken);

        var rapprochees = bulletin.Cours.Select(c =>
        {
            titres.TryGetValue(c.Isin, out var titre);
            var probleme = titre is null
                ? "Titre inconnu : créez-le d'abord."
                : titre.TypeTitre != c.Section
                    ? $"Le bulletin le cote comme {c.Section.ToLowerInvariant()}, la base comme {titre.TypeTitre.ToLowerInvariant()}."
                    : null;
            return (Cours: c, Titre: titre, Probleme: probleme);
        }).ToList();

        var controles = await ControleCouponCouru.ControlerAsync(calculs, rapprochees
            .Select(r => r.Probleme is null && r.Titre?.Obligation is { } o && r.Cours.CouponCouruPublie is { } publie
                ? new ObligationAControler(o, bulletin.DateSeance, publie)
                : null)
            .ToList(), cancellationToken);

        var lignes = rapprochees.Select((r, i) => new LigneBulletinDto(
                r.Cours.Isin, r.Titre?.Nom ?? r.Cours.Libelle, r.Cours.Section, r.Titre?.Id,
                r.Cours.CoursCloture, r.Cours.Volume, r.Cours.CouponCouruPublie, controles[i], r.Probleme))
            .ToList();

        var lus = bulletin.Cours.Select(c => c.Isin).ToHashSet(StringComparer.Ordinal);
        var absents = titres.Values
            .Where(t => !lus.Contains(t.Isin) && t.Statut == nameof(StatutTitre.Actif))
            .OrderBy(t => t.Nom, StringComparer.CurrentCulture)
            .Select(t => new TitreAbsentDto(t.Isin, t.Nom, t.TypeTitre))
            .ToList();

        var importable = bulletin.Anomalies.Count == 0 && lignes.Count > 0 && lignes.All(l => l.Probleme is null) && dejaEnregistres == 0;

        return Result<BulletinAnalyseDto>.Succes(new BulletinAnalyseDto(
            bulletin.Numero, bulletin.DateSeance, dejaEnregistres, lignes, bulletin.Anomalies, absents, importable));
    }
}
