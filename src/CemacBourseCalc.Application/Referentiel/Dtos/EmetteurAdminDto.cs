namespace CemacBourseCalc.Application.Referentiel.Dtos;

/// <summary>Un émetteur dans la liste de l'administration, avec le nombre de ses titres.</summary>
public sealed record EmetteurAdminDto(Guid Id, string Nom, string? Sigle, string Type, string Pays, string? SiteWeb, int NombreTitres);
