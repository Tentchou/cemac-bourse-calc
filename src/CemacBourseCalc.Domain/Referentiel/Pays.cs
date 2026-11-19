using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Referentiel;

/// <summary>Un pays de la CEMAC. La liste est fixe : elle est créée par la migration, jamais saisie.</summary>
public sealed class Pays
{
    public PaysCemac Code { get; private set; }
    public string Libelle { get; private set; } = null!;

    private Pays() { } // EF Core

    public Pays(PaysCemac code)
    {
        Code = code;
        Libelle = PaysCemacLibelles.Libelle(code);
    }
}
