using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteur;
using CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteursEnMasse;
using CemacBourseCalc.Application.Referentiel.Dtos;
using CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteurs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

[Route("api/admin/emetteurs")]
[Authorize(Policy = SecuriteAdmin.PolitiqueSaisie)]
public sealed class EmetteursAdminController(ISender mediator) : ControleurAdmin
{
    /// <summary>GET api/admin/emetteurs — la liste, pour les choix des formulaires et l'import de fichier.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmetteurResumeDto>>> Lister(CancellationToken ct) =>
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
