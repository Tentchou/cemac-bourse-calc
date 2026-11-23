namespace CemacBourseCalc.Application.Place.Dtos;

/// <summary>
/// Un jour férié calculé pour l'année à préparer. DejaEnregistre : la date est déjà dans le calendrier
/// (elle ne sera pas renvoyée) ; LibelleEnregistre : le libellé qu'elle y porte.
/// </summary>
public sealed record JourProposeDto(DateOnly Date, string Libelle, bool DejaEnregistre, string? LibelleEnregistre);

/// <summary>
/// L'aperçu d'une année : les jours fériés certains du Cameroun, et les jours déjà saisis cette année-là
/// qui ne viennent pas du calcul (fêtes musulmanes, fermetures exceptionnelles annoncées par avis).
/// </summary>
public sealed record AnneePrepareeDto(int Annee, IReadOnlyList<JourProposeDto> Proposes, IReadOnlyList<JourNonOuvreDto> AutresEnregistres);
