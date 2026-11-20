using System.Security.Claims;
using CemacBourseCalc.Application.Administration.Interfaces;

namespace CemacBourseCalc.Api.Securite;

/// <summary>L'administrateur authentifié par le cookie de session ; null hors requête HTTP (ligne de commande).</summary>
public sealed class AdministrateurCourant(IHttpContextAccessor accesseur) : IAdministrateurCourant
{
    public Guid? Id =>
        accesseur.HttpContext?.User is { Identity.IsAuthenticated: true } utilisateur
        && Guid.TryParse(utilisateur.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : null;
}
