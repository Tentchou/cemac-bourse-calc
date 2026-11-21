using CemacBourseCalc.Application.Bulletins.Common;
using CemacBourseCalc.Application.Bulletins.Interfaces;
using CemacBourseCalc.Application.Common;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig;

namespace CemacBourseCalc.Infrastructure.Pdf;

/// <summary>
/// Lecture d'un PDF avec PdfPig (code managé, sans exécuter le contenu du PDF) : seulement le texte et sa
/// position. Le fichier reste en mémoire ; un PDF chiffré, abîmé ou trop long est refusé avec un message clair.
/// </summary>
public sealed class LecteurPdfPig(ILogger<LecteurPdfPig> journal) : ILecteurPdf
{
    public Result<IReadOnlyList<PageBulletin>> Lire(byte[] contenu, int pagesMaximum, Func<IReadOnlyList<PageBulletin>, bool>? suffisant = null)
    {
        try
        {
            using var document = PdfDocument.Open(contenu, new ParsingOptions { UseLenientParsing = false });
            if (document.NumberOfPages > pagesMaximum)
                return Result<IReadOnlyList<PageBulletin>>.Echec($"Le PDF compte {document.NumberOfPages} pages : un bulletin de la cote en a une trentaine.");

            var pages = new List<PageBulletin>(document.NumberOfPages);
            for (var numero = 1; numero <= document.NumberOfPages; numero++)
            {
                var page = document.GetPage(numero);
                var hauteur = page.Height;
                var mots = page.GetWords()
                    .Where(m => !string.IsNullOrWhiteSpace(m.Text))
                    .Select(m => new MotPdf(
                        m.BoundingBox.Left,
                        m.BoundingBox.Right,
                        hauteur - m.BoundingBox.Bottom,
                        m.BoundingBox.Height,
                        m.Text))
                    .ToList();
                pages.Add(new PageBulletin(page.Number, mots));
                if (suffisant?.Invoke(pages) == true) break;
            }
            return Result<IReadOnlyList<PageBulletin>>.Succes(pages);
        }
        catch (Exception e) when (e is not OutOfMemoryException)
        {
            // Le détail technique reste dans le journal du serveur ; l'administrateur reçoit un message simple.
            journal.LogWarning(e, "PDF illisible ({Taille} octets).", contenu.Length);
            return Result<IReadOnlyList<PageBulletin>>.Echec("Le PDF est illisible (fichier abîmé, protégé ou d'un autre format).");
        }
    }
}
