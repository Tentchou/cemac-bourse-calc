namespace CemacBourseCalc.Api.Securite;

/// <summary>
/// En-têtes de sécurité sur chaque réponse (site et API). Le site ne charge rien d'extérieur :
/// scripts, styles, polices et images viennent tous de la même adresse.
/// </summary>
public sealed class EnTetesSecurite(RequestDelegate suivant)
{
    // style-src 'unsafe-inline' : quelques styles posés par React (couleurs, animations) ; aucun script en ligne n'est permis.
    private const string Politique =
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data: blob:; " +
        "font-src 'self' data:; connect-src 'self'; object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'";

    public Task InvokeAsync(HttpContext contexte)
    {
        var enTetes = contexte.Response.Headers;
        enTetes.ContentSecurityPolicy = Politique;
        enTetes.XContentTypeOptions = "nosniff";
        enTetes.XFrameOptions = "DENY";
        enTetes["Referrer-Policy"] = "strict-origin-when-cross-origin";
        enTetes["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
        enTetes["Cross-Origin-Opener-Policy"] = "same-origin";
        return suivant(contexte);
    }
}
