namespace CemacBourseCalc.Application.Bulletins.Common;

/// <summary>
/// Un mot lu dans le PDF et sa place sur la page, en points, origine en haut à gauche :
/// X début, X2 fin, Y ligne de base (vers le bas), Hauteur du texte.
/// </summary>
public sealed record MotPdf(double X, double X2, double Y, double Hauteur, string Texte);

/// <summary>Une page du PDF : son numéro (à partir de 1) et ses mots.</summary>
public sealed record PageBulletin(int Numero, IReadOnlyList<MotPdf> Mots);
