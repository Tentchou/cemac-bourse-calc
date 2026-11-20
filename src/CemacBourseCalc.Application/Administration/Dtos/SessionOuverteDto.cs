using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Application.Administration.Dtos;

/// <summary>
/// Le jeton n'est rendu qu'une fois, à l'ouverture : l'API le place dans un cookie HttpOnly et ne
/// l'écrit jamais dans le corps de la réponse. Seule son empreinte est en base.
/// </summary>
public sealed record SessionOuverteDto(string Jeton, DateTimeOffset ExpireLe, string Nom, RoleAdministrateur Role);
