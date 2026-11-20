namespace CemacBourseCalc.Api.Securite;

/// <summary>
/// Protection contre les requêtes forgées (CSRF), en plus du cookie SameSite=Strict : toute écriture
/// sur l'administration doit porter l'en-tête <see cref="SecuriteAdmin.EnTeteCsrf"/>. Un autre site ne
/// peut pas l'ajouter : le navigateur exigerait une autorisation CORS, que l'API ne lui donne pas.
/// </summary>
public sealed class ProtectionCsrf(RequestDelegate suivant)
{
    public async Task InvokeAsync(HttpContext contexte)
    {
        var requete = contexte.Request;
        var ecriture = !(HttpMethods.IsGet(requete.Method) || HttpMethods.IsHead(requete.Method) || HttpMethods.IsOptions(requete.Method));

        if (ecriture
            && requete.Path.StartsWithSegments(SecuriteAdmin.CheminAdmin, StringComparison.OrdinalIgnoreCase)
            && requete.Headers[SecuriteAdmin.EnTeteCsrf] != "1")
        {
            contexte.Response.StatusCode = StatusCodes.Status403Forbidden;
            await Results.Problem(title: "Requête refusée.", statusCode: StatusCodes.Status403Forbidden).ExecuteAsync(contexte);
            return;
        }

        await suivant(contexte);
    }
}
