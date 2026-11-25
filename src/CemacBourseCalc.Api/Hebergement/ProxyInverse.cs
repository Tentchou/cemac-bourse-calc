using Microsoft.AspNetCore.HttpOverrides;

namespace CemacBourseCalc.Api.Hebergement;

/// <summary>
/// En ligne, l'API est derrière un proxy (le relais Cloudflare, puis celui de l'hébergeur) : la connexion
/// vient du proxy, pas du visiteur. Sans ce réglage, la limitation des requêtes compterait tous les
/// visiteurs comme une seule adresse IP.
///
/// Désactivé par défaut (en local, personne ne doit pouvoir choisir son adresse par un en-tête).
/// Activé au déploiement par <c>ProxyInverse__Actif=true</c>.
/// <list type="bullet">
/// <item><c>EnTeteIp</c> : l'en-tête qui porte l'adresse du visiteur. Derrière le relais Cloudflare,
/// <c>X-CBC-IP-Visiteur</c>, posé par le relais seul (voir front/relais/relais.js) ; une requête qui
/// ne vient pas du relais est refusée avant (<see cref="AccesParRelais"/>).</item>
/// <item><c>Sauts</c> : nombre d'éléments lus à partir de la fin de la liste, ceux ajoutés par les
/// proxys de confiance, jamais ceux qu'un visiteur aurait écrits lui-même.</item>
/// </list>
/// </summary>
public static class ProxyInverse
{
    public const string Section = "ProxyInverse";

    public static IServiceCollection AddProxyInverse(this IServiceCollection services, IConfiguration configuration)
    {
        var sauts = configuration.GetValue($"{Section}:Sauts", 1);
        var enTeteIp = configuration.GetValue<string>($"{Section}:EnTeteIp");
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            // L'adresse seule : X-Forwarded-Proto vient d'un autre proxy (l'hébergeur) que l'adresse (le relais),
            // et des listes de longueurs différentes feraient tout ignorer. HTTPS est garanti par Cloudflare.
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
            options.ForwardLimit = sauts;
            if (!string.IsNullOrWhiteSpace(enTeteIp)) options.ForwardedForHeaderName = enTeteIp;
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
