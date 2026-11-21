using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteur;
using CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteursEnMasse;
using CemacBourseCalc.Application.Referentiel.Dtos;
using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteurs;
using CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteursPage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

[Route("api/admin/emetteurs")]
[Authorize(Policy = SecuriteAdmin.PolitiqueSaisie)]
public sealed class EmetteursAdminController(ISender mediator) : ControleurAdmin
{
    /// <summary>GET api/admin/emetteurs?page&amp;taille&amp;recherche&amp;pays&amp;type — une page de la liste.</summary>
    [HttpGet]
    public async Task<ActionResult<PageDto<EmetteurAdminDto>>> Lister(
        [FromQuery] int page = 1, [FromQuery] int taille = 10, [FromQuery] string? recherche = null,
        [FromQuery] string? pays = null, [FromQuery] string? type = null, CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListerEmetteursPageQuery(page, taille, recherche, pays, type), ct));

    /// <summary>GET api/admin/emetteurs/choix — tous les émetteurs, pour les choix des formulaires et l'import de fichier.</summary>
    [HttpGet("choix")]
    public async Task<ActionResult<IReadOnlyList<EmetteurResumeDto>>> Choix(CancellationToken ct) =>
        Ok(await mediator.Send(new ListerEmetteursQuery(), ct));

    /// <summary>POST api/admin/emetteurs — un émetteur.</summary>
    [HttpPost]
    public async Task<ActionResult> Ajouter(AjouterEmetteurCommand commande, CancellationToken ct) =>
        Cree(await mediator.Send(commande, ct));

    /// <summary>POST api/admin/emetteurs/masse { lignes: [...] } — tout ou rien.</summary>
    [HttpPost("masse")]
    [RequestSizeLimit(TailleMasseMaximum)]
    public async Task<ActionResult> AjouterEnMasse(AjouterEmetteursEnMasseCommand commande, CancellationToken ct) =>
        Masse(await mediator.Send(commande, ct));
}
