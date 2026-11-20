using System.Security.Claims;
using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Administration.Commands.FermerSession;
using CemacBourseCalc.Application.Administration.Commands.OuvrirSession;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

/// <summary>
/// Connexion, déconnexion et « qui suis-je » de l'administration. Le jeton de session voyage
/// uniquement dans un cookie HttpOnly : il n'apparaît jamais dans le corps d'une réponse.
/// </summary>
[Route("api/admin/session")]
public sealed class SessionController(ISender mediator) : ControleurAdmin
{
    public sealed record SessionReponse(string Nom, string Role, DateTimeOffset ExpireLe);
    public sealed record MoiReponse(Guid Id, string Nom, string Email, string Role);

    /// <summary>POST api/admin/session { email, motDePasse } — limité à quelques essais par minute et par IP.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<SessionReponse>> Ouvrir(OuvrirSessionCommand commande, CancellationToken ct)
    {
        var resultat = await mediator.Send(commande, ct);
        if (!resultat.Reussi) return Problem(title: resultat.Message, statusCode: StatusCodes.Status401Unauthorized);

        var session = resultat.Valeur!;
        Response.Cookies.Append(SecuriteAdmin.Cookie, session.Jeton, SecuriteAdmin.OptionsCookie(session.ExpireLe));
        return Ok(new SessionReponse(session.Nom, session.Role.ToString(), session.ExpireLe));
    }

    /// <summary>DELETE api/admin/session — révoque la session en base et efface le cookie.</summary>
    [HttpDelete]
    [AllowAnonymous]
    public async Task<IActionResult> Fermer(CancellationToken ct)
    {
        if (Request.Cookies.TryGetValue(SecuriteAdmin.Cookie, out var jeton) && !string.IsNullOrEmpty(jeton))
            await mediator.Send(new FermerSessionCommand(jeton), ct);

        Response.Cookies.Delete(SecuriteAdmin.Cookie, SecuriteAdmin.OptionsCookie(DateTimeOffset.UnixEpoch));
        return NoContent();
    }

    /// <summary>GET api/admin/session — l'administrateur connecté (401 si la session n'est plus valide).</summary>
    [HttpGet]
    [Authorize(Policy = SecuriteAdmin.PolitiqueSaisie)]
    public ActionResult<MoiReponse> Moi() => Ok(new MoiReponse(
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
        User.FindFirstValue(ClaimTypes.Name)!,
        User.FindFirstValue(ClaimTypes.Email)!,
        User.FindFirstValue(ClaimTypes.Role)!));
}
