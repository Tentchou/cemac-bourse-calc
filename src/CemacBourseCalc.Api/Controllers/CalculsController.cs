using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Calculs.Queries.CalculerCouponCouru;
using CemacBourseCalc.Application.Calculs.Queries.CalculerRendementObligation;
using CemacBourseCalc.Application.Calculs.Queries.ObtenirEcheancier;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers;

/// <summary>Calculs publics (lecture seule, soumis à la limitation des requêtes).</summary>
[ApiController]
[Route("api/obligations/{titreId:guid}")]
public sealed class CalculsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// GET api/obligations/{titreId}/coupon-couru?dateNegociation=2026-01-12&amp;quantite=10&amp;cours=98.5
    /// Le cours est facultatif : sans lui, seul le coupon couru est calculé.
    /// </summary>
    [HttpGet("coupon-couru")]
    public async Task<ActionResult<CalculCouponCouruDto>> CouponCouru(
        Guid titreId, [FromQuery] DateOnly dateNegociation, [FromQuery] int quantite = 1, [FromQuery] decimal? cours = null,
        CancellationToken ct = default)
    {
        var resultat = await mediator.Send(new CalculerCouponCouruQuery(titreId, dateNegociation, quantite, cours), ct);
        return resultat.Reussi
            ? Ok(resultat.Valeur)
            : Problem(title: resultat.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// GET api/obligations/{titreId}/echeancier?quantite=10&amp;dateNegociation=2026-01-12
    /// Sans date : l'échéancier complet. Avec une date : les échéances que l'acheteur percevra.
    /// </summary>
    [HttpGet("echeancier")]
    public async Task<ActionResult<EcheancierDto>> Echeancier(
        Guid titreId, [FromQuery] int quantite = 1, [FromQuery] DateOnly? dateNegociation = null,
        CancellationToken ct = default)
    {
        var resultat = await mediator.Send(new ObtenirEcheancierQuery(titreId, quantite, dateNegociation), ct);
        return resultat.Reussi
            ? Ok(resultat.Valeur)
            : Problem(title: resultat.Message, statusCode: StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// GET api/obligations/{titreId}/rendement?dateNegociation=2026-01-12&amp;cours=98.5
    /// Rendement courant et rendement actuariel, pour un titre, à partir du règlement.
    /// </summary>
    [HttpGet("rendement")]
    public async Task<ActionResult<RendementObligationDto>> Rendement(
        Guid titreId, [FromQuery] DateOnly dateNegociation, [FromQuery] decimal cours, CancellationToken ct = default)
    {
        var resultat = await mediator.Send(new CalculerRendementObligationQuery(titreId, dateNegociation, cours), ct);
        return resultat.Reussi
            ? Ok(resultat.Valeur)
            : Problem(title: resultat.Message, statusCode: StatusCodes.Status400BadRequest);
    }
}
