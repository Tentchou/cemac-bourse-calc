namespace CemacBourseCalc.Domain.Communs;

/// <summary>Les six pays de la CEMAC, par leur code ISO 3166-1 (préfixe de l'ISIN).</summary>
public enum PaysCemac
{
    CM, // Cameroun
    CF, // République centrafricaine
    CG, // Congo
    GA, // Gabon
    GQ, // Guinée équatoriale
    TD  // Tchad
}

public static class PaysCemacLibelles
{
    public static string Libelle(PaysCemac pays) => pays switch
    {
        PaysCemac.CM => "Cameroun",
        PaysCemac.CF => "République centrafricaine",
        PaysCemac.CG => "Congo",
        PaysCemac.GA => "Gabon",
        PaysCemac.GQ => "Guinée équatoriale",
        PaysCemac.TD => "Tchad",
        _ => pays.ToString()
    };
}
