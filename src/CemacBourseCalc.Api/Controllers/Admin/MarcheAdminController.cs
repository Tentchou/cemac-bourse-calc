using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Queries.ListerCoursAdmin;
using CemacBourseCalc.Application.Titres.Queries.ListerDividendesAdmin;
using CemacBourseCalc.Application.Titres.Queries.ListerSeances;
using CemacBourseCalc.Application.Titres.Commands.AjouterCours;
using CemacBourseCalc.Application.Titres.Commands.AjouterCoursEnMasse;
using CemacBourseCalc.Application.Titres.Commands.AjouterDividende;
using CemacBourseCalc.Application.Titres.Commands.AjouterDividendesEnMasse;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

/// <summary>Les données de marché : cours des séances et dividendes versés.</summary>
[Route("api/admin")]
[Authorize(Policy = SecuriteAdmin.PolitiqueSaisie)]
public sealed class MarcheAdminController(ISender mediator) : ControleurAdmin
{
    /// <summary>GET api/admin/cours?page&amp;taille&amp;recherche&amp;seance — les cours d'une séance (la dernière par défaut).</summary>
    [HttpGet("cours")]
    public async Task<ActionResult<PageDto<CoursAdminDto>>> ListerCours(
        [FromQuery] int page = 1, [FromQuery] int taille = 10, [FromQuery] string? recherche = null,
        [FromQuery] DateOnly? seance = null, CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListerCoursAdminQuery(page, taille, recherche, seance), ct));

    /// <summary>GET api/admin/cours/seances — les dernières séances saisies, de la plus récente à la plus ancienne.</summary>
    [HttpGet("cours/seances")]
    public async Task<ActionResult<IReadOnlyList<DateOnly>>> ListerSeances(CancellationToken ct) =>
        Ok(await mediator.Send(new ListerSeancesQuery(), ct));

    /// <summary>GET api/admin/dividendes?page&amp;taille&amp;recherche — une page.</summary>
    [HttpGet("dividendes")]
    public async Task<ActionResult<PageDto<DividendeAdminDto>>> ListerDividendes(
        [FromQuery] int page = 1, [FromQuery] int taille = 10, [FromQuery] string? recherche = null, CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListerDividendesAdminQuery(page, taille, recherche), ct));

    /// <summary>POST api/admin/cours — le cours d'un titre pour une séance.</summary>
    [HttpPost("cours")]
    public async Task<ActionResult> AjouterCours(AjouterCoursCommand commande, CancellationToken ct) =>
        Cree(await mediator.Send(commande, ct));

    /// <summary>POST api/admin/cours/masse { lignes: [...] } — un bulletin de cote entier, tout ou rien.</summary>
    [HttpPost("cours/masse")]
    [RequestSizeLimit(TailleMasseMaximum)]
    public async Task<ActionResult> AjouterCoursEnMasse(AjouterCoursEnMasseCommand commande, CancellationToken ct) =>
        Masse(await mediator.Send(commande, ct));

    /// <summary>POST api/admin/dividendes — un versement de dividende.</summary>
    [HttpPost("dividendes")]
    public async Task<ActionResult> AjouterDividende(AjouterDividendeCommand commande, CancellationToken ct) =>
        Cree(await mediator.Send(commande, ct));

    /// <summary>POST api/admin/dividendes/masse { lignes: [...] } — tout ou rien.</summary>
    [HttpPost("dividendes/masse")]
    [RequestSizeLimit(TailleMasseMaximum)]
    public async Task<ActionResult> AjouterDividendesEnMasse(AjouterDividendesEnMasseCommand commande, CancellationToken ct) =>
        Masse(await mediator.Send(commande, ct));
}
