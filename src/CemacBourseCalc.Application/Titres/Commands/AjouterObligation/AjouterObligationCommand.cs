using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterObligation;

/// <summary>Saisie d'une obligation par l'administrateur, d'après son prospectus.</summary>
public sealed record AjouterObligationCommand(
    string Isin,
    string Nom,
    string Emetteur,
    decimal Nominal,
    decimal TauxFacial,
    Periodicite Periodicite,
    DateOnly DateJouissance,
    DateOnly DateEcheance,
    ModeRemboursement ModeRemboursement,
    int DiffereEcheances,
    string CodeConvention) : IRequest<Result<int>>;
