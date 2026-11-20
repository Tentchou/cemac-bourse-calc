namespace CemacBourseCalc.Application.Administration.Common;

/// <summary>Les libellés du journal de l'administration : une seule écriture pour chaque action et chaque entité.</summary>
public static class Journal
{
    public const string Creation = "Creation";
    public const string Connexion = "Connexion";
    public const string Deconnexion = "Deconnexion";

    public const string EntiteAdministrateur = "administrateur";
    public const string EntiteEmetteur = "emetteur";
    public const string EntiteTitre = "titre";
    public const string EntiteCours = "cours";
    public const string EntiteDividende = "dividende";
    public const string EntiteJourNonOuvre = "jour_non_ouvre";

    public const string MessageSessionRequise = "Votre session a expiré : reconnectez-vous.";
}
