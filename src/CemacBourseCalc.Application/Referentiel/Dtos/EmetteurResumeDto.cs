namespace CemacBourseCalc.Application.Referentiel.Dtos;

/// <summary>Un émetteur dans les listes de l'administration (choix dans un formulaire, import de fichier).</summary>
public sealed record EmetteurResumeDto(Guid Id, string Nom, string? Sigle, string Type, string Pays);
