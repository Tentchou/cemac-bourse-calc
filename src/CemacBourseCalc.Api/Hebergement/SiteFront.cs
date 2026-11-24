using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace CemacBourseCalc.Api.Hebergement;

/// <summary>
/// En ligne, l'API sert aussi le site React compilé (copié dans wwwroot par le Dockerfile) :
/// une seule adresse, donc le cookie de session SameSite=Strict et la protection CSRF marchent tels quels.
/// En développement, wwwroot n'existe pas : Vite sert le site et relaie /api, rien ne change.
/// </summary>
public static class SiteFront
{
    private const string PageApplication = "index.html";

    /// <summary>Les fichiers de /assets portent une empreinte dans leur nom : ils ne changent jamais.</summary>
    private const string DossierEmpreinte = "/assets";

    private static bool Present(WebApplication app) => app.Environment.WebRootFileProvider.GetFileInfo(PageApplication).Exists;

    public static WebApplication UseSiteFront(this WebApplication app)
    {
        if (!Present(app)) return app;

        app.UseDefaultFiles();
        app.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = Cache });
        return app;
    }

    /// <summary>
    /// Après les contrôleurs : une route /api inconnue reste une 404 de l'API ; toute autre adresse
    /// (/titres/…, /methodes, /admin) rend l'application, dont le routeur affiche la bonne page.
    /// </summary>
    public static WebApplication MapSiteFront(this WebApplication app)
    {
        if (!Present(app)) return app;

        app.MapFallback("/api/{**chemin}", () => Results.Problem(title: "Route inconnue.", statusCode: StatusCodes.Status404NotFound));
        app.MapFallbackToFile(PageApplication, new StaticFileOptions { OnPrepareResponse = Cache });
        return app;
    }

    private static void Cache(StaticFileResponseContext contexte)
    {
        var enTetes = contexte.Context.Response.GetTypedHeaders();
        enTetes.CacheControl = contexte.Context.Request.Path.StartsWithSegments(DossierEmpreinte)
            ? new CacheControlHeaderValue { Public = true, MaxAge = TimeSpan.FromDays(365), Extensions = { new NameValueHeaderValue("immutable") } }
            : new CacheControlHeaderValue { NoCache = true };   // index.html : toujours revalidé, pour voir la dernière version
    }
}
