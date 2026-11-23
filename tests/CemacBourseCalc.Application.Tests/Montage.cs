using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Place.Interfaces;
using CemacBourseCalc.Infrastructure.Securite;
using Microsoft.Extensions.DependencyInjection;

namespace CemacBourseCalc.Application.Tests;

/// <summary>La vraie couche Application (MediatR, validateurs, handlers) branchée sur la fausse base.</summary>
internal static class Montage
{
    /// <param name="completer">Ajoute les faux propres à un test (écriture des titres…), une fois la fausse base créée.</param>
    public static (IServiceProvider Services, FausseBase Base) Creer(Guid? administrateurConnecte, Action<IServiceCollection, FausseBase>? completer = null)
    {
        var base_ = new FausseBase();
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddSingleton<IUniteDeTravail>(base_);
        services.AddSingleton<IJoursNonOuvresEcriture>(new FauxJoursNonOuvres(base_));
        services.AddSingleton<IJournalAdmin>(new FauxJournal(base_));
        services.AddSingleton<IAdministrateursEcriture>(new FauxAdministrateurs(base_));
        services.AddSingleton<IAdministrateurCourant>(new FauxAdministrateurCourant(administrateurConnecte));
        services.AddSingleton<IHacheurMotDePasse, HacheurMotDePasse>();
        completer?.Invoke(services, base_);
        return (services.BuildServiceProvider(), base_);
    }
}
