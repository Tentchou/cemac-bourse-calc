using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Administration.Commands.CreerAdministrateur;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

/// <summary>Gestion des comptes : réservée au rôle Admin.</summary>
[Route("api/admin/administrateurs")]
[Authorize(Policy = SecuriteAdmin.PolitiqueAdmin)]
public sealed class AdministrateursController(ISender mediator) : ControleurAdmin
{
    /// <summary>POST api/admin/administrateurs { email, nom, motDePasse, role }</summary>
    [HttpPost]
    public async Task<ActionResult> Creer(CreerAdministrateurCommand commande, CancellationToken ct) =>
        Cree(await mediator.Send(commande, ct));
}
