using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Administration.Dtos;
using CemacBourseCalc.Application.Administration.Queries.ObtenirCompteurs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

/// <summary>Les compteurs du menu de l'administration.</summary>
[Route("api/admin/compteurs")]
[Authorize(Policy = SecuriteAdmin.PolitiqueSaisie)]
public sealed class CompteursAdminController(ISender mediator) : ControleurAdmin
{
    /// <summary>GET api/admin/compteurs — le nombre de lignes de chaque rubrique.</summary>
    [HttpGet]
    public async Task<ActionResult<CompteursAdminDto>> Obtenir(CancellationToken ct) =>
        Ok(await mediator.Send(new ObtenirCompteursQuery(), ct));
}
