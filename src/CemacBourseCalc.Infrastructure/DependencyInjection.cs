using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Bulletins.Interfaces;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Place.Interfaces;
using CemacBourseCalc.Application.Referentiel.Interfaces;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Infrastructure.Ecriture;
using CemacBourseCalc.Infrastructure.Lecture;
using CemacBourseCalc.Infrastructure.Pdf;
using CemacBourseCalc.Infrastructure.Persistence;
using CemacBourseCalc.Infrastructure.Securite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace CemacBourseCalc.Infrastructure;

public static class DependencyInjection
{
    public const string NomChaineConnexion = "CemacBourseCalc";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var chaine = configuration.GetConnectionString(NomChaineConnexion);
        if (string.IsNullOrWhiteSpace(chaine))
            throw new InvalidOperationException(
                $"La chaîne de connexion « {NomChaineConnexion} » est absente. En développement : dotnet user-secrets set \"ConnectionStrings:{NomChaineConnexion}\" \"Host=…\"");

        // Une seule source de connexions (pool Npgsql), partagée par EF Core et Dapper.
        var source = new NpgsqlDataSourceBuilder(chaine).Build();
        services.AddSingleton(source);

        services.AddDbContext<CalcDbContext>(options => options
            .UseNpgsql(source)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUniteDeTravail, UniteDeTravail>();
        services.AddScoped<IJournalAdmin, JournalAdminEcriture>();
        services.AddSingleton<IHacheurMotDePasse, HacheurMotDePasse>();

        services.AddScoped<IAdministrateursEcriture, AdministrateursEcriture>();
        services.AddScoped<ISessionsLecture, SessionsLecture>();
        services.AddScoped<IEmetteursEcriture, EmetteursEcriture>();
        services.AddScoped<IEmetteursLecture, EmetteursLecture>();
        services.AddScoped<IJoursNonOuvresEcriture, JoursNonOuvresEcriture>();
        services.AddScoped<ITitresEcriture, TitresEcriture>();
        services.AddScoped<ITitresLecture, TitresLecture>();
        services.AddScoped<IHistoriqueCoursLecture, HistoriqueCoursLecture>();
        services.AddScoped<ILectureCalculs, LectureCalculs>();
        services.AddScoped<ITitresAdminLecture, TitresAdminLecture>();
        services.AddScoped<IMarcheAdminLecture, MarcheAdminLecture>();
        services.AddScoped<IJoursNonOuvresLecture, JoursNonOuvresLecture>();
        services.AddScoped<ICompteursLecture, CompteursLecture>();
        services.AddScoped<IBulletinsLecture, BulletinsLecture>();
        services.AddSingleton<ILecteurPdf, LecteurPdfPig>();
        return services;
    }
}
