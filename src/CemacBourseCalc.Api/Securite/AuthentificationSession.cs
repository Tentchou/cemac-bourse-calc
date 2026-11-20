using System.Security.Claims;
using System.Text.Encodings.Web;
using CemacBourseCalc.Application.Administration.Queries.ObtenirAdministrateurDeSession;
using CemacBourseCalc.Domain.Communs;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CemacBourseCalc.Api.Securite;

/// <summary>Les noms partagés de la sécurité de l'administration.</summary>
public static class SecuriteAdmin
{
    public const string Schema = "SessionAdmin";
    public const string Cookie = "cbc_session";
    public const string CheminAdmin = "/api/admin";

    /// <summary>En-tête exigé sur toute écriture de l'administration (protection CSRF, voir <see cref="ProtectionCsrf"/>).</summary>
    public const string EnTeteCsrf = "X-CBC-Admin";

    /// <summary>Saisie des données : Admin et Editeur.</summary>
    public const string PolitiqueSaisie = "Saisie";

    /// <summary>Gestion des comptes : Admin seulement.</summary>
    public const string PolitiqueAdmin = "Admin";

    public static CookieOptions OptionsCookie(DateTimeOffset expireLe) => new()
    {
        HttpOnly = true,              // illisible par JavaScript : une faille XSS ne vole pas la session
        Secure = true,                // jamais envoyé en clair (les navigateurs l'acceptent sur http://localhost)
        SameSite = SameSiteMode.Strict,
        Path = CheminAdmin,           // envoyé seulement aux routes de l'administration
        Expires = expireLe,
        IsEssential = true
    };
}

/// <summary>
/// Lit le cookie de session et vérifie en base que la session est ouverte et le compte actif.
/// Une session révoquée (déconnexion, compte désactivé) est refusée dès la requête suivante.
/// </summary>
public sealed class AuthentificationSession(
    IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISender mediator)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.TryGetValue(SecuriteAdmin.Cookie, out var jeton) || string.IsNullOrEmpty(jeton))
            return AuthenticateResult.NoResult();

        var admin = await mediator.Send(new ObtenirAdministrateurDeSessionQuery(jeton), Context.RequestAborted);
        if (admin is null) return AuthenticateResult.Fail("Session invalide ou expirée.");

        var identite = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new Claim(ClaimTypes.Name, admin.Nom),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Role, admin.Role.ToString())
        ], SecuriteAdmin.Schema);

        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identite), SecuriteAdmin.Schema));
    }
}

public static class ConfigurationSecurite
{
    public static IServiceCollection AddSecuriteAdmin(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<Application.Administration.Interfaces.IAdministrateurCourant, AdministrateurCourant>();

        services.AddAuthentication(SecuriteAdmin.Schema)
            .AddScheme<AuthenticationSchemeOptions, AuthentificationSession>(SecuriteAdmin.Schema, null);

        services.AddAuthorizationBuilder()
            .AddPolicy(SecuriteAdmin.PolitiqueSaisie, p => p
                .RequireAuthenticatedUser()
                .RequireRole(nameof(RoleAdministrateur.Admin), nameof(RoleAdministrateur.Editeur)))
            .AddPolicy(SecuriteAdmin.PolitiqueAdmin, p => p
                .RequireAuthenticatedUser()
                .RequireRole(nameof(RoleAdministrateur.Admin)));

        return services;
    }
}
