using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Place.Commands.AjouterJourNonOuvre;

/// <summary>Un jour sans séance à la BVMAC (férié, fermeture exceptionnelle). Rend la date enregistrée.</summary>
public sealed record AjouterJourNonOuvreCommand(DateOnly Date, string Libelle) : IRequest<Result<DateOnly>>;
