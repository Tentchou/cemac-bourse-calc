using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application.Bulletins.Dtos;
using CemacBourseCalc.Application.Bulletins.Queries.AnalyserBulletin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

/// <summary>Le bulletin officiel de la cote : lecture du PDF et aperçu, avant l'import des cours.</summary>
[Route("api/admin/bulletins")]
[Authorize(Policy = SecuriteAdmin.PolitiqueSaisie)]
public sealed class BulletinsAdminController(ISender mediator) : ControleurAdmin
{
    /// <summary>Marge pour l'enveloppe multipart autour du fichier.</summary>
    private const long TailleRequeteMaximum = AnalyserBulletinQuery.TailleMaximum + 64 * 1024;

    /// <summary>
    /// POST api/admin/bulletins/analyse (multipart, champ « fichier ») — lit le PDF et rend l'aperçu.
    /// Rien n'est enregistré : l'import se fait ensuite par api/admin/cours/masse, comme un fichier CSV.
    /// </summary>
    [HttpPost("analyse")]
    [RequestSizeLimit(TailleRequeteMaximum)]
    [RequestFormLimits(MultipartBodyLengthLimit = TailleRequeteMaximum)]
    public async Task<ActionResult<BulletinAnalyseDto>> Analyser(IFormFile? fichier, CancellationToken ct)
    {
        if (fichier is null || fichier.Length == 0)
            return Problem(title: "Choisissez le fichier PDF du bulletin.", statusCode: StatusCodes.Status400BadRequest);
        if (fichier.Length > AnalyserBulletinQuery.TailleMaximum)
            return Problem(title: "Le fichier dépasse 10 Mo : ce n'est pas un bulletin de la cote.", statusCode: StatusCodes.Status400BadRequest);

        // En mémoire seulement (10 Mo au plus) : le fichier n'est jamais écrit sur le disque du serveur.
        using var memoire = new MemoryStream((int)fichier.Length);
        await fichier.CopyToAsync(memoire, ct);

        var resultat = await mediator.Send(new AnalyserBulletinQuery(memoire.ToArray()), ct);
        return resultat.Reussi
            ? Ok(resultat.Valeur)
            : Problem(title: resultat.Message, statusCode: StatusCodes.Status400BadRequest);
    }
}
