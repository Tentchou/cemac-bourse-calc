namespace CemacBourseCalc.Application.Place.Dtos;

/// <summary>Un jour sans séance à la BVMAC.</summary>
public sealed record JourNonOuvreDto(DateOnly Date, string Libelle);
