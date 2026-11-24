using Microsoft.AspNetCore.HttpOverrides;

namespace CemacBourseCalc.Api.Hebergement;

/// <summary>
/// En ligne, l'API est derrière le proxy de l'hébergeur (Cloud Run) : la connexion vient du proxy,
/// pas du visiteur. Sans ce réglage, la limitation des requêtes compterait tous les visiteurs
/// comme une seule adresse IP, et l'API croirait la requête en HTTP.
///
/// Désactivé par défaut (en local, personne ne doit pouvoir choisir son adresse par un en-tête).
/// Activé au déploiement par <c>ProxyInverse__Actif=true</c>. Seuls les <c>Sauts</c> derniers
/// éléments de <c>X-Forwarded-For</c> sont lus : ceux ajoutés par le proxy, pas ceux qu'un visiteur
/// aurait écrits lui-même.
/// </summary>
public static class ProxyInverse
{
    public const string Section = "ProxyInverse";

    public static IServiceCollection AddProxyInverse(this IServiceCollection services, IConfiguration configuration)
    {
        var sauts = configuration.GetValue($"{Section}:Sauts", 1);
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = sauts;
            // Le conteneur n'est joignable que par le proxy de l'hébergeur, dont les adresses ne sont pas fixes.
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
        return services;
    }

    public static IApplicationBuilder UseProxyInverse(this WebApplication app)
    {
        if (app.Configuration.GetValue($"{Section}:Actif", false)) app.UseForwardedHeaders();
        return app;
    }
}
