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
        if (nombreTitres <= 0) throw new DomainException("Le nombre de titres de la société doit être supérieur à 0.");
        NombreTitres = nombreTitres;
    }
}
