namespace CemacBourseCalc.Application.Calculs.Dtos;

/// <summary>Rendement du dividende d'une action sur le dernier exercice connu, en %. Montants en FCFA.</summary>
public sealed record RendementDividendeDto(
    string Titre,
    decimal Cours,
    int Exercice,
    int NombreVersements,
    decimal DividendeBrut,
    decimal? DividendeNet,
    decimal RendementBrut,
    decimal? RendementNet);
