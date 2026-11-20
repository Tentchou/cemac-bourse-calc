using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterAction;

/// <summary>Saisie d'une action cotée par l'administrateur. Rend l'identifiant du titre créé.</summary>
public sealed record AjouterActionCommand(
    string Isin,
    string Nom,
    Guid EmetteurId,
    decimal Nominal,
    DateOnly? DateAdmission,
    long NombreTitres) : IRequest<Result<Guid>>;
