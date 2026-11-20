using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Commands.CreerAdministrateur;

/// <summary>
/// Création d'un compte de l'administration : par un Admin connecté, ou par la ligne de commande du
/// serveur pour le tout premier compte (<c>dotnet run -- creer-admin</c>).
/// </summary>
public sealed record CreerAdministrateurCommand(string Email, string Nom, string MotDePasse, RoleAdministrateur Role)
    : IRequest<Result<Guid>>
{
    public override string ToString() => $"{nameof(CreerAdministrateurCommand)} {{ Email = {Email}, Nom = {Nom}, Role = {Role} }}";
}
