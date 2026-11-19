using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Queries.ListerTitres;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers;

/// <summary>Contrôleur fin : il transmet la requête à MediatR et rend le résultat.</summary>
[ApiController]
[Route("api/titres")]
public sealed class TitresController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TitreResumeDto>>> Lister(CancellationToken ct) =>
        Ok(await mediator.Send(new ListerTitresQuery(), ct));
}
