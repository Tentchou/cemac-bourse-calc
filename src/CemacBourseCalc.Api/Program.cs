using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CemacBourseCalc.Api.Erreurs;
using CemacBourseCalc.Api.Hebergement;
using CemacBourseCalc.Api.Securite;
using CemacBourseCalc.Application;
using CemacBourseCalc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSecuriteAdmin();
builder.Services.AddProxyInverse(builder.Configuration);

// Les énumérations circulent par leur nom (« Semestrielle », « Admin »), comme en base.
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GestionnaireErreurs>();

// Limitation des requêtes : l'API est publique. Fenêtre fixe par adresse IP.
const string PolitiquePublique = "publique";
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy(PolitiquePublique, contexte => RateLimitPartition.GetFixedWindowLimiter(
        contexte.Connection.RemoteIpAddress?.ToString() ?? "inconnue",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = builder.Configuration.GetValue("LimiteRequetes:ParMinute", 60),
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));

    // En plus : peu d'essais de connexion par minute et par IP, contre la recherche de mots de passe.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(contexte =>
        HttpMethods.IsPost(contexte.Request.Method)
        && contexte.Request.Path.Equals($"{SecuriteAdmin.CheminAdmin}/session", StringComparison.OrdinalIgnoreCase)
            ? RateLimitPartition.GetFixedWindowLimiter(
                "connexion:" + (contexte.Connection.RemoteIpAddress?.ToString() ?? "inconnue"),
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = builder.Configuration.GetValue("LimiteRequetes:ConnexionsParMinute", 5),
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                })
            : RateLimitPartition.GetNoLimiter("libre"));
});

const string PolitiqueFront = "front";
builder.Services.AddCors(options => options.AddPolicy(PolitiqueFront, politique => politique
    .WithOrigins(builder.Configuration.GetSection("Front:Origines").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));

var app = builder.Build();

// Création d'un compte depuis le serveur (le premier compte en particulier) : voir le README.
if (args is [CreationAdministrateurConsole.Commande, ..])
    return await CreationAdministrateurConsole.ExecuterAsync(app.Services);

// En premier : l'adresse du visiteur et le schéma (https) viennent du proxy de l'hébergeur, s'il y en a un.
app.UseProxyInverse();
app.UseExceptionHandler();
app.UseMiddleware<EnTetesSecurite>();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
else app.UseHsts();

app.UseHttpsRedirection();
app.UseSiteFront();
app.UseCors(PolitiqueFront);
app.UseRateLimiter();
app.UseMiddleware<ProtectionCsrf>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting(PolitiquePublique);
app.MapSiteFront();

app.Run();
return 0;
