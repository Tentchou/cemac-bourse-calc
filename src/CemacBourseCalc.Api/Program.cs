using System.Threading.RateLimiting;
using CemacBourseCalc.Api.Erreurs;
using CemacBourseCalc.Application;
using CemacBourseCalc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
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
});

const string PolitiqueFront = "front";
builder.Services.AddCors(options => options.AddPolicy(PolitiqueFront, politique => politique
    .WithOrigins(builder.Configuration.GetSection("Front:Origines").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseCors(PolitiqueFront);
app.UseRateLimiter();

app.MapControllers().RequireRateLimiting(PolitiquePublique);

app.Run();
