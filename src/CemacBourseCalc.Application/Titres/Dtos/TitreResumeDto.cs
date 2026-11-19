namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>Une ligne de la liste publique des titres.</summary>
public sealed record TitreResumeDto(int Id, string Isin, string Nom, string Emetteur, string Pays, string TypeTitre, decimal Nominal);
