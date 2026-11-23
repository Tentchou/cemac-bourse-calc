using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Queries.ListerActionsAdmin;
using CemacBourseCalc.Application.Titres.Queries.ListerObligationsAdmin;
using CemacBourseCalc.Application.Titres.Commands.AjouterAction;
using CemacBourseCalc.Application.Titres.Commands.AjouterActionsEnMasse;
using CemacBourseCalc.Application.Titres.Commands.AjouterObligation;
using CemacBourseCalc.Application.Titres.Commands.AjouterObligationsEnMasse;
using CemacBourseCalc.Application.Titres.Commands.ModifierAction;
using CemacBourseCalc.Application.Titres.Commands.ModifierObligation;
using CemacBourseCalc.Application.Titres.Commands.SupprimerTitre;
using CemacBourseCalc.Application.Titres.Queries.ObtenirTitreAdmin;
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

    /// <summary>GET api/admin/titres/{id} — un titre complet pour le formulaire de modification, avec la taille de son historique.</summary>
    [HttpGet("titres/{id:guid}")]
    public async Task<ActionResult<TitreAdminDetailDto>> Detail(Guid id, CancellationToken ct) =>
        await mediator.Send(new ObtenirTitreAdminQuery(id), ct) is { } titre
            ? Ok(titre)
            : Problem(title: "Ce titre est introuvable.", statusCode: StatusCodes.Status404NotFound);

    /// <summary>PUT api/admin/obligations/{id} — corrige une obligation (tout sauf l'ISIN et le type).</summary>
    [HttpPut("obligations/{id:guid}")]
    public async Task<ActionResult> ModifierObligation(Guid id, ModifierObligationCommand commande, CancellationToken ct) =>
        Fait(await mediator.Send(commande with { Id = id }, ct));

    /// <summary>PUT api/admin/actions/{id} — corrige une action (tout sauf l'ISIN et le type).</summary>
    [HttpPut("actions/{id:guid}")]
    public async Task<ActionResult> ModifierAction(Guid id, ModifierActionCommand commande, CancellationToken ct) =>
        Fait(await mediator.Send(commande with { Id = id }, ct));

    /// <summary>DELETE api/admin/titres/{id} — efface un titre sans historique, radie un titre qui en a un.</summary>
    [HttpDelete("titres/{id:guid}")]
    public async Task<ActionResult> Supprimer(Guid id, CancellationToken ct) =>
        Fait(await mediator.Send(new SupprimerTitreCommand(id), ct));
}
