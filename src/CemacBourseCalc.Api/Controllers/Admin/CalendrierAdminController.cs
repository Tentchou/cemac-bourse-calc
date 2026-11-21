using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Place.Dtos;
using CemacBourseCalc.Application.Place.Queries.ListerJoursNonOuvres;
using CemacBourseCalc.Application.Place.Commands.AjouterJourNonOuvre;
using CemacBourseCalc.Application.Place.Commands.AjouterJoursNonOuvresEnMasse;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

/// <summary>Le calendrier de la BVMAC : les jours sans séance (hors week-ends).</summary>
[Route("api/admin/jours-non-ouvres")]
[Authorize(Policy = SecuriteAdmin.PolitiqueSaisie)]
public sealed class CalendrierAdminController(ISender mediator) : ControleurAdmin
{
    /// <summary>GET api/admin/jours-non-ouvres?page&amp;taille&amp;recherche&amp;annee — une page, du plus récent au plus ancien.</summary>
    [HttpGet]
    public async Task<ActionResult<PageDto<JourNonOuvreDto>>> Lister(
        [FromQuery] int page = 1, [FromQuery] int taille = 10, [FromQuery] string? recherche = null,
        [FromQuery] int? annee = null, CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListerJoursNonOuvresQuery(page, taille, recherche, annee), ct));

    /// <summary>POST api/admin/jours-non-ouvres { date, libelle }</summary>
    [HttpPost]
    public async Task<ActionResult> Ajouter(AjouterJourNonOuvreCommand commande, CancellationToken ct) =>
        Cree(await mediator.Send(commande, ct));

    /// <summary>POST api/admin/jours-non-ouvres/masse { lignes: [...] } — le calendrier d'une année, tout ou rien.</summary>
    [HttpPost("masse")]
    [RequestSizeLimit(TailleMasseMaximum)]
    public async Task<ActionResult> AjouterEnMasse(AjouterJoursNonOuvresEnMasseCommand commande, CancellationToken ct) =>
        Masse(await mediator.Send(commande, ct));
}
