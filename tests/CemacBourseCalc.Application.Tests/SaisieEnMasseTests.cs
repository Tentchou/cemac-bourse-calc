using CemacBourseCalc.Application.Common.Masse;
using CemacBourseCalc.Application.Place.Commands.AjouterJourNonOuvre;
using CemacBourseCalc.Application.Place.Commands.AjouterJoursNonOuvresEnMasse;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CemacBourseCalc.Application.Tests;

/// <summary>
/// La saisie en masse, testée sur le calendrier (la commande la plus simple) : le moteur est le même
/// pour les émetteurs, titres, cours et dividendes.
/// </summary>
public class SaisieEnMasseTests
{
    private static readonly Guid Admin = Guid.CreateVersion7();
    private static DateOnly D(int a, int m, int j) => new(a, m, j);

    private static async Task<(ResultatMasseDto Bilan, FausseBase Base)> Envoyer(params AjouterJourNonOuvreCommand[] lignes)
    {
        var (services, base_) = Montage.Creer(Admin);
        var resultat = await services.GetRequiredService<ISender>().Send(new AjouterJoursNonOuvresEnMasseCommand(lignes));
        Assert.True(resultat.Reussi);
        return (resultat.Valeur!, base_);
    }

    [Fact]
    public async Task Tout_est_valide_tout_est_enregistre_avec_le_journal()
    {
        var (bilan, base_) = await Envoyer(
            new(D(2026, 1, 1), "Jour de l'an"),
            new(D(2026, 5, 1), "Fête du travail"),
            new(D(2026, 12, 25), "Noël"));

        Assert.True(bilan.Complet);
        Assert.Equal(3, bilan.Enregistrees);
        Assert.True(base_.TransactionValidee);
        Assert.Equal(3, base_.JoursEnregistres.Count);
        Assert.Equal(3, base_.Journal.Count);
        Assert.All(base_.Journal, j => Assert.Equal(Admin, j.Auteur));
    }

    [Fact]
    public async Task Une_seule_erreur_et_rien_n_est_enregistre()
    {
        // Le 03/01/2026 est un samedi : la saisie manuelle le refuse, la masse aussi.
        var (bilan, base_) = await Envoyer(
            new(D(2026, 1, 1), "Jour de l'an"),
            new(D(2026, 1, 3), "Samedi"),
            new(D(2026, 12, 25), "Noël"));

        Assert.False(bilan.Complet);
        Assert.Equal(0, bilan.Enregistrees);
        var erreur = Assert.Single(bilan.Erreurs);
        Assert.Equal(2, erreur.Ligne);
        Assert.Contains("week-end", erreur.Messages.Single());

        Assert.True(base_.TransactionAnnulee);
        Assert.Empty(base_.JoursEnregistres);
        Assert.Empty(base_.Journal);
    }

    [Fact]
    public async Task Toutes_les_erreurs_sont_rendues_en_une_fois()
    {
        var (bilan, _) = await Envoyer(
            new(D(2026, 1, 1), ""),                 // validateur : libellé vide
            new(D(2026, 1, 3), "Samedi"),           // handler : week-end
            new(D(2026, 5, 1), "Fête du travail"),
            new(D(2026, 5, 1), "Fête du travail")); // doublon dans l'envoi

        Assert.Equal([1, 2, 4], bilan.Erreurs.Select(e => e.Ligne));
        Assert.Contains("ligne 3", bilan.Erreurs[2].Messages.Single());
    }

    [Fact]
    public async Task Une_date_deja_en_base_est_refusee_comme_en_saisie_manuelle()
    {
        var (services, base_) = Montage.Creer(Admin);
        base_.JoursEnregistres.Add(D(2026, 12, 25));

        var resultat = await services.GetRequiredService<ISender>().Send(new AjouterJoursNonOuvresEnMasseCommand(
            [new(D(2026, 1, 1), "Jour de l'an"), new(D(2026, 12, 25), "Noël")]));

        var erreur = Assert.Single(resultat.Valeur!.Erreurs);
        Assert.Equal(2, erreur.Ligne);
        Assert.Contains("déjà dans le calendrier", erreur.Messages.Single());
        Assert.Equal([D(2026, 12, 25)], base_.JoursEnregistres); // le 01/01 n'est pas resté
    }

    [Theory]
    [InlineData(0)]
    [InlineData(LignesMasse.Maximum + 1)]
    public async Task Un_envoi_vide_ou_trop_gros_est_refuse(int nombre)
    {
        var (services, _) = Montage.Creer(Admin);
        var lignes = Enumerable.Range(0, nombre).Select(i => new AjouterJourNonOuvreCommand(D(2027, 1, 1).AddDays(i), "Test")).ToList();

        await Assert.ThrowsAsync<ValidationException>(() =>
            services.GetRequiredService<ISender>().Send(new AjouterJoursNonOuvresEnMasseCommand(lignes)));
    }

    [Fact]
    public async Task Sans_session_aucune_ligne_ne_passe()
    {
        var (services, base_) = Montage.Creer(administrateurConnecte: null);
        var resultat = await services.GetRequiredService<ISender>().Send(
            new AjouterJoursNonOuvresEnMasseCommand([new(D(2026, 1, 1), "Jour de l'an")]));

        Assert.False(resultat.Valeur!.Complet);
        Assert.Empty(base_.JoursEnregistres);
    }
}
