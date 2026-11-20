using System.Text;
using CemacBourseCalc.Application.Administration.Commands.CreerAdministrateur;
using CemacBourseCalc.Domain.Communs;
using FluentValidation;
using MediatR;

namespace CemacBourseCalc.Api.Securite;

/// <summary>
/// Création d'un compte depuis le serveur : <c>dotnet run --project src/CemacBourseCalc.Api -- creer-admin</c>.
/// C'est la seule façon de créer le premier compte : aucune route publique ne le permet.
/// Le mot de passe est tapé au clavier, masqué, et n'est écrit nulle part.
/// </summary>
public static class CreationAdministrateurConsole
{
    public const string Commande = "creer-admin";

    public static async Task<int> ExecuterAsync(IServiceProvider services)
    {
        Console.WriteLine("Création d'un compte de l'administration CEMAC Bourse Calc.");
        var email = Demander("E-mail : ");
        var nom = Demander("Nom : ");
        var role = Demander("Rôle (Admin / Editeur) [Admin] : ") is { Length: > 0 } r ? r : nameof(RoleAdministrateur.Admin);
        if (!Enum.TryParse<RoleAdministrateur>(role, ignoreCase: true, out var roleChoisi))
        {
            Console.Error.WriteLine("Rôle inconnu : tapez Admin ou Editeur.");
            return 1;
        }

        var motDePasse = DemanderMasque("Mot de passe (12 caractères au moins) : ");
        if (motDePasse != DemanderMasque("Confirmez le mot de passe : "))
        {
            Console.Error.WriteLine("Les deux mots de passe sont différents. Rien n'a été créé.");
            return 1;
        }

        await using var portee = services.CreateAsyncScope();
        var mediator = portee.ServiceProvider.GetRequiredService<ISender>();
        try
        {
            var resultat = await mediator.Send(new CreerAdministrateurCommand(email, nom, motDePasse, roleChoisi));
            if (!resultat.Reussi)
            {
                Console.Error.WriteLine(resultat.Message);
                return 1;
            }
            Console.WriteLine(resultat.Message);
            return 0;
        }
        catch (ValidationException v)
        {
            foreach (var erreur in v.Errors.Select(e => e.ErrorMessage).Distinct()) Console.Error.WriteLine(erreur);
            return 1;
        }
    }

    private static string Demander(string invite)
    {
        Console.Write(invite);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static string DemanderMasque(string invite)
    {
        Console.Write(invite);
        var saisie = new StringBuilder();
        while (true)
        {
            var touche = Console.ReadKey(intercept: true);
            if (touche.Key == ConsoleKey.Enter) break;
            if (touche.Key == ConsoleKey.Backspace)
            {
                if (saisie.Length > 0) saisie.Length--;
                continue;
            }
            if (!char.IsControl(touche.KeyChar)) saisie.Append(touche.KeyChar);
        }
        Console.WriteLine();
        return saisie.ToString();
    }
}
