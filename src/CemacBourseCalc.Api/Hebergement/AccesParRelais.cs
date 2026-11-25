using System.Security.Cryptography;
using System.Text;

namespace CemacBourseCalc.Api.Hebergement;

/// <summary>
/// En ligne, le site est sur Cloudflare Pages et relaie /api vers l'API (front/relais/relais.js).
/// Quand <c>Relais:Secret</c> est configuré, l'API refuse toute requête qui ne porte pas ce secret :
/// on ne peut pas l'appeler directement à son adresse d'hébergement pour contourner la limitation
/// des requêtes ou inventer son adresse IP. Sans secret configuré (en local), rien n'est filtré.
/// </summary>
public sealed class AccesParRelais(RequestDelegate suivant, IConfiguration configuration)
{
    public const string Section = "Relais";

    /// <summary>Même nom que EN_TETE_SECRET dans front/relais/relais.js.</summary>
    public const string EnTete = "X-CBC-Relais";

    private readonly byte[]? _secret = configuration.GetValue<string>($"{Section}:Secret") is { Length: > 0 } s
        ? Encoding.UTF8.GetBytes(s)
        : null;

    public async Task InvokeAsync(HttpContext contexte)
    {
        if (_secret is not null && !Valide(contexte.Request.Headers[EnTete].ToString()))
        {
            contexte.Response.StatusCode = StatusCodes.Status403Forbidden;
            await Results.Problem(title: "Accès direct refusé.", statusCode: StatusCodes.Status403Forbidden).ExecuteAsync(contexte);
            return;
        }

        await suivant(contexte);
    }

    /// <summary>Comparaison en temps constant : la durée ne révèle rien du secret.</summary>
    private bool Valide(string recu) =>
        recu.Length > 0 && CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(recu), _secret);
}
