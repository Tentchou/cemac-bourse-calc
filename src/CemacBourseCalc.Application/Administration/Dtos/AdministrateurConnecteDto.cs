using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Application.Administration.Dtos;

public sealed record AdministrateurConnecteDto(Guid Id, string Nom, string Email, RoleAdministrateur Role);
