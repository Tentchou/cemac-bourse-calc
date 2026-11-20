using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Calculs.Queries.CalculerRendementDividende;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers;

/// <summary>Calculs publics sur les actions (lecture seule, soumis à la limitation des requêtes).</summary>
[ApiController]
[Route("api/actions/{titreId:guid}")]
public sealed class CalculsActionsController(ISender mediator) : ControllerBase
{
    /// <summary>GET api/actions/{titreId}/rendement-dividende?cours=32000 (cours en FCFA).</summary>
    [HttpGet("rendement-dividende")]
    public async Task<ActionResult<RendementDividendeDto>> RendementDividende(
        Guid titreId, [FromQuery] decimal cours, CancellationToken ct = default)
    {
        var resultat = await mediator.Send(new CalculerRendementDividendeQuery(titreId, cours), ct);
        return resultat.Reussi
            ? Ok(resultat.Valeur)
            : Problem(title: resultat.Message, statusCode: StatusCodes.Status400BadRequest);
    }
}
