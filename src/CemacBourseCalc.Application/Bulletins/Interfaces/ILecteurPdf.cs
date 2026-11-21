using CemacBourseCalc.Application.Bulletins.Common;
using CemacBourseCalc.Application.Common;

namespace CemacBourseCalc.Application.Bulletins.Interfaces;

/// <summary>Extraction des mots d'un PDF, avec leur position. Le fichier n'est jamais écrit sur le disque.</summary>
public interface ILecteurPdf
{
    /// <summary>
    /// Les pages et leurs mots, ou un message si le fichier est illisible ou dépasse <paramref name="pagesMaximum"/> pages.
    /// <paramref name="suffisant"/> : appelé après chaque page avec les pages déjà lues ; vrai = inutile de lire la suite.
    /// </summary>
    Result<IReadOnlyList<PageBulletin>> Lire(byte[] contenu, int pagesMaximum, Func<IReadOnlyList<PageBulletin>, bool>? suffisant = null);
}
