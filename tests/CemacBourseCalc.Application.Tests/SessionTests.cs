using CemacBourseCalc.Application.Administration.Commands.CreerAdministrateur;
using CemacBourseCalc.Application.Administration.Commands.FermerSession;
using CemacBourseCalc.Application.Administration.Commands.OuvrirSession;
using CemacBourseCalc.Application.Administration.Common;
using CemacBourseCalc.Domain.Communs;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CemacBourseCalc.Application.Tests;

public class SessionTests
{
    private const string MotDePasse = "une phrase de passe assez longue";

    private static async Task<(ISender Mediator, FausseBase Base)> AvecUnCompte()
    {
        var (services, base_) = Montage.Creer(administrateurConnecte: null);
        var mediator = services.GetRequiredService<ISender>();
        var cree = await mediator.Send(new CreerAdministrateurCommand("Romeo@Exemple.cm", "Roméo", MotDePasse, RoleAdministrateur.Admin));
        Assert.True(cree.Reussi);
        return (mediator, base_);
    }

    [Fact]
    public async Task Le_mot_de_passe_n_est_jamais_stocke_en_clair()
    {
        var (_, base_) = await AvecUnCompte();
        var admin = Assert.Single(base_.Administrateurs);

        Assert.Equal("romeo@exemple.cm", admin.Email);
        Assert.DoesNotContain(MotDePasse, admin.MotDePasseHash);
        // Créé depuis la ligne de commande : l'auteur du journal est le compte lui-même.
        Assert.Equal(admin.Id, Assert.Single(base_.Journal).Auteur);
    }

    [Fact]
    public async Task Connexion_reussie_le_jeton_n_est_stocke_que_par_son_empreinte()
    {
        var (mediator, base_) = await AvecUnCompte();

        var r = await mediator.Send(new OuvrirSessionCommand("romeo@exemple.cm", MotDePasse));

        Assert.True(r.Reussi);
        Assert.True(JetonsSession.FormatValide(r.Valeur!.Jeton));
        var session = Assert.Single(base_.Sessions);
        Assert.NotEqual(r.Valeur.Jeton, session.JetonHash);
        Assert.Equal(JetonsSession.Empreinte(r.Valeur.Jeton), session.JetonHash);
        Assert.Equal(JetonsSession.Duree, session.ExpireLe - session.CreeLe);
    }

    [Fact]
    public async Task E_mail_inconnu_et_mot_de_passe_faux_donnent_le_meme_message()
    {
        var (mediator, base_) = await AvecUnCompte();

        var inconnu = await mediator.Send(new OuvrirSessionCommand("personne@exemple.cm", MotDePasse));
        var faux = await mediator.Send(new OuvrirSessionCommand("romeo@exemple.cm", "pas le bon mot de passe"));

        Assert.False(inconnu.Reussi);
        Assert.False(faux.Reussi);
        Assert.Equal(inconnu.Message, faux.Message);
        Assert.Empty(base_.Sessions);
    }

    [Fact]
    public async Task La_deconnexion_revoque_la_session()
    {
        var (mediator, base_) = await AvecUnCompte();
        var r = await mediator.Send(new OuvrirSessionCommand("romeo@exemple.cm", MotDePasse));

        await mediator.Send(new FermerSessionCommand(r.Valeur!.Jeton));

        Assert.NotNull(Assert.Single(base_.Sessions).RevoqueLe);
    }

    [Fact]
    public async Task Un_mot_de_passe_trop_court_est_refuse()
    {
        var (services, _) = Montage.Creer(administrateurConnecte: null);
        await Assert.ThrowsAsync<ValidationException>(() => services.GetRequiredService<ISender>()
            .Send(new CreerAdministrateurCommand("a@exemple.cm", "A", "court", RoleAdministrateur.Editeur)));
    }

    [Fact]
    public async Task Deux_comptes_ne_partagent_pas_un_e_mail()
    {
        var (mediator, _) = await AvecUnCompte();
        var doublon = await mediator.Send(new CreerAdministrateurCommand("ROMEO@exemple.cm", "Autre", MotDePasse, RoleAdministrateur.Editeur));
        Assert.False(doublon.Reussi);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("trop-court")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa!")] // 43 caractères mais un caractère interdit
    public void Un_jeton_mal_forme_est_refuse_sans_requete(string? jeton)
    {
        Assert.False(JetonsSession.FormatValide(jeton));
    }
}
