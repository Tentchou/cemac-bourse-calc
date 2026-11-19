using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterObligation;

/// <summary>Saisie d'une obligation par l'administrateur, d'après son prospectus. Rend l'identifiant du titre créé.</summary>
public sealed record AjouterObligationCommand(
    string Isin,
    string Nom,
    Guid EmetteurId,
    decimal Nominal,
    DateOnly? DateAdmission,
    decimal TauxFacial,
    Periodicite Periodicite,
    DateOnly DateJouissance,
    DateOnly? DatePremierCoupon,
    DateOnly DateEcheance,
    ModeRemboursement ModeRemboursement,
    int DiffereEcheances,
    string CodeConvention,
    RegimeFiscal RegimeFiscal) : IRequest<Result<Guid>>;
