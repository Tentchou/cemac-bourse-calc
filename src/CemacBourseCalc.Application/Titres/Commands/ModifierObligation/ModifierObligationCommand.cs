using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.ModifierObligation;

/// <summary>
/// Correction d'une obligation par l'administrateur : tout sauf l'ISIN et le type, statut compris.
/// L'identifiant vient de l'adresse (PUT api/admin/obligations/{id}), jamais du corps.
/// </summary>
public sealed record ModifierObligationCommand(
    Guid Id,
    string Nom,
    Guid EmetteurId,
    decimal Nominal,
    DateOnly? DateAdmission,
    StatutTitre Statut,
    decimal TauxFacial,
    Periodicite Periodicite,
    DateOnly DateJouissance,
    DateOnly? DatePremierCoupon,
    DateOnly DateEcheance,
    ModeRemboursement ModeRemboursement,
    int DiffereEcheances,
    string CodeConvention,
    RegimeFiscal RegimeFiscal) : IRequest<Result<Guid>>;
