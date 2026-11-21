using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Queries.ListerTitres;
using CemacBourseCalc.Application.Titres.Queries.ObtenirFicheTitre;
using CemacBourseCalc.Application.Titres.Queries.ObtenirHistoriqueCours;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers;

/// <summary>Contrôleur fin : il transmet la requête à MediatR et rend le résultat.</summary>
[ApiController]
[Route("api/titres")]
public sealed class TitresController(ISender mediator) : ControllerBase
{
    /// <summary>GET api/titres : la liste publique, avec taux, échéance et dernier cours.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TitreResumeDto>>> Lister(CancellationToken ct) =>
        Ok(await mediator.Send(new ListerTitresQuery(), ct));

    /// <summary>GET api/titres/{id} : la fiche d'un titre (caractéristiques, dernier cours, dividendes).</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FicheTitreDto>> Fiche(Guid id, CancellationToken ct) =>
        await mediator.Send(new ObtenirFicheTitreQuery(id), ct) is { } fiche
            ? Ok(fiche)
            : Problem(title: "Ce titre est introuvable.", statusCode: StatusCodes.Status404NotFound);

    /// <summary>
    /// GET api/titres/{id}/cours : l'historique des cours (environ cinq ans de séances), du plus ancien au plus récent.
    /// </summary>
    [HttpGet("{id:guid}/cours")]
    public async Task<ActionResult<IReadOnlyList<PointCoursDto>>> Historique(Guid id, CancellationToken ct) =>
        await mediator.Send(new ObtenirHistoriqueCoursQuery(id), ct) is { } points
            ? Ok(points)
            : Problem(title: "Ce titre est introuvable.", statusCode: StatusCodes.Status404NotFound);
}
