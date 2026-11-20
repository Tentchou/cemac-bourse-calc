using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Titres.Commands.AjouterAction;
using CemacBourseCalc.Application.Titres.Commands.AjouterActionsEnMasse;
using CemacBourseCalc.Application.Titres.Commands.AjouterObligation;
using CemacBourseCalc.Application.Titres.Commands.AjouterObligationsEnMasse;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

[Route("api/admin")]
[Authorize(Policy = SecuriteAdmin.PolitiqueSaisie)]
public sealed class TitresAdminController(ISender mediator) : ControleurAdmin
{
    /// <summary>POST api/admin/obligations — une obligation, d'après son prospectus.</summary>
    [HttpPost("obligations")]
    public async Task<ActionResult> AjouterObligation(AjouterObligationCommand commande, CancellationToken ct) =>
        Cree(await mediator.Send(commande, ct));

    /// <summary>POST api/admin/obligations/masse { lignes: [...] } — tout ou rien.</summary>
    [HttpPost("obligations/masse")]
    [RequestSizeLimit(TailleMasseMaximum)]
    public async Task<ActionResult> AjouterObligationsEnMasse(AjouterObligationsEnMasseCommand commande, CancellationToken ct) =>
        Masse(await mediator.Send(commande, ct));

    /// <summary>POST api/admin/actions — une action cotée.</summary>
    [HttpPost("actions")]
    public async Task<ActionResult> AjouterAction(AjouterActionCommand commande, CancellationToken ct) =>
        Cree(await mediator.Send(commande, ct));

    /// <summary>POST api/admin/actions/masse { lignes: [...] } — tout ou rien.</summary>
    [HttpPost("actions/masse")]
    [RequestSizeLimit(TailleMasseMaximum)]
    public async Task<ActionResult> AjouterActionsEnMasse(AjouterActionsEnMasseCommand commande, CancellationToken ct) =>
        Masse(await mediator.Send(commande, ct));
}
