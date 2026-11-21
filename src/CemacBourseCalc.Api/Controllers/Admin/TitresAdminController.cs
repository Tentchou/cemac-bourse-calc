using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Queries.ListerActionsAdmin;
using CemacBourseCalc.Application.Titres.Queries.ListerObligationsAdmin;
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
    /// <summary>GET api/admin/obligations?page&amp;taille&amp;recherche&amp;pays&amp;remboursement — une page, avec le contrôle du coupon couru.</summary>
    [HttpGet("obligations")]
    public async Task<ActionResult<PageDto<ObligationAdminDto>>> ListerObligations(
        [FromQuery] int page = 1, [FromQuery] int taille = 10, [FromQuery] string? recherche = null,
        [FromQuery] string? pays = null, [FromQuery] string? remboursement = null, CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListerObligationsAdminQuery(page, taille, recherche, pays, remboursement), ct));

    /// <summary>GET api/admin/actions?page&amp;taille&amp;recherche&amp;pays — une page.</summary>
    [HttpGet("actions")]
    public async Task<ActionResult<PageDto<ActionAdminDto>>> ListerActions(
        [FromQuery] int page = 1, [FromQuery] int taille = 10, [FromQuery] string? recherche = null,
        [FromQuery] string? pays = null, CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListerActionsAdminQuery(page, taille, recherche, pays), ct));

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
