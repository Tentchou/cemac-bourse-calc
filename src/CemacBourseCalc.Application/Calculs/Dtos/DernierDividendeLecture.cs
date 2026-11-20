namespace CemacBourseCalc.Application.Calculs.Dtos;

/// <summary>
/// Une action et la somme des versements de son dernier exercice. Exercice null : aucun dividende saisi.
/// Net null : au moins un versement n'a pas de montant net connu.
/// </summary>
public sealed record DernierDividendeLecture(string Nom, int? Exercice, decimal? Brut, decimal? Net, int NombreVersements);
