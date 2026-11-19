using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CemacBourseCalc.Api.Erreurs;

/// <summary>
/// Une saisie refusée par un validateur devient une réponse 400 avec les messages en français.
/// Toute autre erreur reste une 500 générique : aucun détail technique ne sort de l'API.
/// </summary>
public sealed class GestionnaireErreurs(IProblemDetailsService problemes) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext contexte, Exception exception, CancellationToken ct)
    {
        if (exception is not ValidationException validation) return false;

        contexte.Response.StatusCode = StatusCodes.Status400BadRequest;
        var details = new ValidationProblemDetails(validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
        {
            Title = "La saisie contient des erreurs.",
            Status = StatusCodes.Status400BadRequest
        };

        return await problemes.TryWriteAsync(new ProblemDetailsContext { HttpContext = contexte, ProblemDetails = details, Exception = exception });
    }
}
