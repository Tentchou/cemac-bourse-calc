using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Titres;

/// <summary>Une action cotée. Le cours et les dividendes viennent des bulletins de cote de la BVMAC.</summary>
public sealed class Action : Titre
{
    public long NombreTitres { get; private set; }

    private Action() { } // EF Core

    public Action(Isin isin, string nom, string emetteur, decimal nominal, long nombreTitres)
        : base(isin, nom, emetteur, nominal)
    {
        if (nombreTitres <= 0) throw new DomainException("Le nombre de titres de la société doit être supérieur à 0.");
        NombreTitres = nombreTitres;
    }
}
