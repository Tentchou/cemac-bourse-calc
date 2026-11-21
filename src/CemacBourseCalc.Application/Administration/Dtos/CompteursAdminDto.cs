namespace CemacBourseCalc.Application.Administration.Dtos;

/// <summary>Le nombre de lignes de chaque rubrique, pour le menu de l'administration.</summary>
public sealed record CompteursAdminDto(int Obligations, int Actions, int Cours, int Dividendes, int Emetteurs, int JoursNonOuvres);
