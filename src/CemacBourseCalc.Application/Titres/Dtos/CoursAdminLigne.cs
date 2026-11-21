using CemacBourseCalc.Domain.Calculs;

namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>Ce que la lecture rend pour un cours : la ligne et, pour une obligation, ses caractéristiques de calcul.</summary>
public sealed record CoursAdminLigne(CoursAdminDto Cours, CaracteristiquesObligation? Caracteristiques);
