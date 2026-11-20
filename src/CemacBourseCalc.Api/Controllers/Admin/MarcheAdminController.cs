using CemacBourseCalc.Api.Securite;
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
