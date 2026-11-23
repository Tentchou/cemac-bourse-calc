using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Application.Titres.Commands.ModifierAction;
using CemacBourseCalc.Application.Titres.Commands.ModifierObligation;
using CemacBourseCalc.Application.Titres.Commands.SupprimerTitre;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Action = CemacBourseCalc.Domain.Titres.Action;

namespace CemacBourseCalc.Application.Tests;

/// <summary>Modifier, effacer ou radier un titre par la vraie couche Application (validateurs, handlers, journal).</summary>
public class ModificationTitresTests
{
    private static readonly Guid Admin = Guid.CreateVersion7();
    private static readonly Guid EmetteurA = Guid.CreateVersion7();
    private static readonly Guid EmetteurB = Guid.CreateVersion7();

    /// <summary>Les titres « en base » : modifiés en place comme des entités suivies, effacés à l'enregistrement.</summary>
    private sealed class FauxTitres(FausseBase base_) : ITitresEcriture
    {
        public List<Titre> Titres { get; } = [];
        public HashSet<Guid> AvecHistorique { get; } = [];

        public Task<bool> IsinExisteAsync(string isin, CancellationToken ct) => Task.FromResult(Titres.Any(t => t.Isin.Valeur == isin));
        public Task<bool> EmetteurExisteAsync(Guid emetteurId, CancellationToken ct) => Task.FromResult(emetteurId == EmetteurA || emetteurId == EmetteurB);
        public Task<TypeTitre?> TypeDuTitreAsync(Guid titreId, CancellationToken ct) => Task.FromResult(Titres.SingleOrDefault(t => t.Id == titreId)?.TypeTitre);
        public Task<bool> CoursExisteAsync(Guid titreId, DateOnly dateSeance, CancellationToken ct) => Task.FromResult(false);
        public Task<Obligation?> ObligationAsync(Guid titreId, CancellationToken ct) => Task.FromResult(Titres.OfType<Obligation>().SingleOrDefault(t => t.Id == titreId));
        public Task<Action?> ActionAsync(Guid titreId, CancellationToken ct) => Task.FromResult(Titres.OfType<Action>().SingleOrDefault(t => t.Id == titreId));
        public Task<Titre?> TitreAsync(Guid titreId, CancellationToken ct) => Task.FromResult(Titres.SingleOrDefault(t => t.Id == titreId));
        public Task<bool> AHistoriqueAsync(Guid titreId, CancellationToken ct) => Task.FromResult(AvecHistorique.Contains(titreId));
        public void Ajouter(Titre titre) => Titres.Add(titre);
        public void Supprimer(Titre titre) => base_.Plus_tard(() => Titres.Remove(titre), () => Titres.Add(titre));
        public void AjouterCours(Cours cours) { }
        public void AjouterDividende(Dividende dividende) { }
    }

    private static Isin IsinCameroun(string onze = "CM000000001") => Isin.Creer(onze + Isin.ChiffreControle(onze));

    private static Obligation Obligation() =>
        new(IsinCameroun(), "Emprunt 6,50 % 2026-2031", EmetteurA, 10_000m, null,
            6.5m, Periodicite.Annuelle, new DateOnly(2026, 6, 30), null, new DateOnly(2031, 6, 30),
            ModeRemboursement.InFine, 0, "EXACT_365", RegimeFiscal.Net);

    private static (ISender Envoi, FausseBase Base, FauxTitres Titres) Monter(Guid? admin, params Titre[] titres)
    {
        FauxTitres faux = null!;
        var (services, base_) = Montage.Creer(admin, (s, b) =>
        {
            faux = new FauxTitres(b);
            s.AddSingleton<ITitresEcriture>(faux);
        });
        faux.Titres.AddRange(titres);
        return (services.GetRequiredService<ISender>(), base_, faux);
    }

    private static ModifierObligationCommand Modification(Obligation o, decimal taux = 6.75m, DateOnly? echeance = null, StatutTitre statut = StatutTitre.Actif) =>
        new(o.Id, "Emprunt 6,75 % 2026-2031", EmetteurB, 10_000m, null, statut, taux, Periodicite.Annuelle,
            new DateOnly(2026, 6, 30), null, echeance ?? new DateOnly(2031, 6, 30), ModeRemboursement.InFine, 0, "EXACT_365", RegimeFiscal.Net);

    [Fact]
    public async Task Une_obligation_est_modifiee_et_le_changement_est_note_au_journal()
    {
        var o = Obligation();
        var (envoi, base_, _) = Monter(Admin, o);

        var resultat = await envoi.Send(Modification(o));

        Assert.True(resultat.Reussi, resultat.Message);
        Assert.Equal(6.75m, o.TauxFacial);
        Assert.Equal(EmetteurB, o.EmetteurId);
        Assert.Equal(IsinCameroun(), o.Isin); // l'ISIN ne change jamais
        var ligne = Assert.Single(base_.Journal);
        Assert.Equal((Admin, Journal.Modification, Journal.EntiteTitre, o.Id.ToString()), ligne);
    }

    [Fact]
    public async Task Sans_changement_rien_n_est_note()
    {
        var o = Obligation();
        var (envoi, base_, _) = Monter(Admin, o);

        var resultat = await envoi.Send(new ModifierObligationCommand(o.Id, o.Nom, o.EmetteurId, o.Nominal, o.DateAdmission, o.Statut,
            o.TauxFacial, o.Periodicite, o.DateJouissance, o.DatePremierCoupon, o.DateEcheance, o.ModeRemboursement,
            o.DiffereEcheances, o.CodeConvention, o.RegimeFiscal));

        Assert.True(resultat.Reussi);
        Assert.Contains("Aucun changement", resultat.Message);
        Assert.Empty(base_.Journal);
    }

    [Fact]
    public async Task Une_regle_du_domaine_refusee_ne_change_rien_et_n_est_pas_notee()
    {
        var o = Obligation();
        var (envoi, base_, _) = Monter(Admin, o);

        var resultat = await envoi.Send(Modification(o, echeance: new DateOnly(2025, 1, 1)));

        Assert.False(resultat.Reussi);
        Assert.Contains("échéance", resultat.Message);
        Assert.Equal(6.5m, o.TauxFacial);
        Assert.Empty(base_.Journal);
    }

    [Fact]
    public async Task Un_statut_inconnu_est_refuse_par_le_validateur()
    {
        var o = Obligation();
        var (envoi, _, _) = Monter(Admin, o);

        await Assert.ThrowsAsync<ValidationException>(() => envoi.Send(Modification(o, statut: (StatutTitre)42)));
    }

    [Fact]
    public async Task Une_action_ne_se_modifie_pas_comme_une_obligation()
    {
        var o = Obligation();
        var (envoi, _, _) = Monter(Admin, o);

        var resultat = await envoi.Send(new ModifierActionCommand(o.Id, "Société", EmetteurA, 10_000m, null, StatutTitre.Actif, 1_000));

        Assert.False(resultat.Reussi);
        Assert.Contains("n'existe plus", resultat.Message);
    }

    [Fact]
    public async Task Sans_session_rien_n_est_modifie()
    {
        var o = Obligation();
        var (envoi, _, _) = Monter(null, o);

        var resultat = await envoi.Send(Modification(o));

        Assert.False(resultat.Reussi);
        Assert.Equal(6.5m, o.TauxFacial);
    }

    [Fact]
    public async Task Un_titre_sans_historique_est_efface()
    {
        var o = Obligation();
        var (envoi, base_, titres) = Monter(Admin, o);

        var resultat = await envoi.Send(new SupprimerTitreCommand(o.Id));

        Assert.True(resultat.Reussi);
        Assert.True(resultat.Valeur!.Efface);
        Assert.Empty(titres.Titres);
        Assert.Equal(Journal.Suppression, Assert.Single(base_.Journal).Action);
    }

    [Fact]
    public async Task Un_titre_avec_historique_est_radie_pas_efface()
    {
        var o = Obligation();
        var (envoi, base_, titres) = Monter(Admin, o);
        titres.AvecHistorique.Add(o.Id);

        var resultat = await envoi.Send(new SupprimerTitreCommand(o.Id));

        Assert.True(resultat.Reussi);
        Assert.False(resultat.Valeur!.Efface);
        Assert.Contains(o, titres.Titres);
        Assert.Equal(StatutTitre.Radie, o.Statut);
        Assert.Equal(Journal.Radiation, Assert.Single(base_.Journal).Action);

        // Une seconde demande ne radie pas deux fois.
        var encore = await envoi.Send(new SupprimerTitreCommand(o.Id));
        Assert.False(encore.Reussi);
        Assert.Contains("déjà radié", encore.Message);
        Assert.Single(base_.Journal);
    }
}
