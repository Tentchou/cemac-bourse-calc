using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>Une action cotée. Le cours et les dividendes viennent des bulletins de cote de la BVMAC.</summary>
public sealed class Action : Titre
{
    public long NombreTitres { get; private set; }

    private Action() { } // EF Core

    public Action(Isin isin, string nom, Guid emetteurId, decimal nominal, DateOnly? dateAdmission, long nombreTitres)
        : base(isin, nom, emetteurId, TypeTitre.Action, nominal, dateAdmission)
    {
        VerifierNombre(nombreTitres);
        NombreTitres = nombreTitres;
    }

    /// <summary>
    /// Corrige l'action (tout sauf l'ISIN et le type), avec les mêmes règles qu'à la création.
    /// Tout est vérifié avant le moindre changement : une modification refusée ne change rien.
    /// </summary>
    public void Modifier(string nom, Guid emetteurId, decimal nominal, DateOnly? dateAdmission, StatutTitre statut, long nombreTitres)
    {
        VerifierCommun(nom, emetteurId, nominal, statut);
        VerifierNombre(nombreTitres);

        AppliquerCommun(nom, emetteurId, nominal, dateAdmission, statut);
        NombreTitres = nombreTitres;
    }

    private static void VerifierNombre(long nombreTitres)
    {
        if (nombreTitres <= 0) throw new DomainException("Le nombre de titres de la société doit être supérieur à 0.");
    }
}
