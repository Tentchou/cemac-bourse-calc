using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterDividende;

/// <summary>Un versement de dividende par action (un acompte et un solde font deux versements du même exercice).</summary>
public sealed record AjouterDividendeCommand(
    Guid TitreId,
    int Exercice,
    decimal MontantBrut,
    decimal? MontantNet,
    DateOnly? DateDetachement,
    DateOnly? DatePaiement) : IRequest<Result<Guid>>;
