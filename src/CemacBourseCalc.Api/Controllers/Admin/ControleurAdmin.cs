using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Application.Common.Masse;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Controllers.Admin;

/// <summary>La traduction commune des résultats de l'administration en réponses HTTP.</summary>
[ApiController]
public abstract class ControleurAdmin : ControllerBase
{
    /// <summary>Taille maximale d'un envoi en masse (500 lignes JSON tiennent largement dans 1 Mo).</summary>
    protected const long TailleMasseMaximum = 1_000_000;

    public sealed record CreationReponse<T>(T Id, string? Message);
    public sealed record MasseReponse(string? Message, ResultatMasseDto Bilan);

    /// <summary>201 avec l'identifiant créé, ou 400 avec le message en français.</summary>
    protected ActionResult Cree<T>(Result<T> resultat) =>
        resultat.Reussi
            ? StatusCode(StatusCodes.Status201Created, new CreationReponse<T>(resultat.Valeur!, resultat.Message))
            : Problem(title: resultat.Message, statusCode: StatusCodes.Status400BadRequest);

    /// <summary>200 si tout est enregistré ; 422 avec la liste des lignes à corriger (et rien d'enregistré).</summary>
    protected ActionResult Masse(Result<ResultatMasseDto> resultat)
    {
        if (!resultat.Reussi) return Problem(title: resultat.Message, statusCode: StatusCodes.Status400BadRequest);
        var bilan = resultat.Valeur!;
        return StatusCode(bilan.Complet ? StatusCodes.Status200OK : StatusCodes.Status422UnprocessableEntity,
            new MasseReponse(resultat.Message, bilan));
    }
}
