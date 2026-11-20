namespace CemacBourseCalc.Application.Administration.Interfaces;

/// <summary>
/// Ajoute une ligne au journal de l'administration. Rien n'est enregistré ici : la ligne part avec
/// l'entité, au même <c>IUniteDeTravail.EnregistrerAsync</c>, pour qu'une action ne soit jamais sans trace.
/// Ne jamais passer de mot de passe ni d'empreinte dans <paramref name="avant"/> ou <paramref name="apres"/>.
/// </summary>
public interface IJournalAdmin
{
    void Noter(Guid auteurId, string action, string entite, string entiteId, object? avant = null, object? apres = null);
}
