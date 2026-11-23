using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.ModifierAction;

/// <summary>
/// Correction d'une action par l'administrateur : tout sauf l'ISIN et le type, statut compris.
/// L'identifiant vient de l'adresse (PUT api/admin/actions/{id}), jamais du corps.
/// </summary>
public sealed record ModifierActionCommand(
    Guid Id,
    string Nom,
    Guid EmetteurId,
    decimal Nominal,
    DateOnly? DateAdmission,
    StatutTitre Statut,
    long NombreTitres) : IRequest<Result<Guid>>;
