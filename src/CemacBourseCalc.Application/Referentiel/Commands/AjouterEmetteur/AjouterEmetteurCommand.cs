using CemacBourseCalc.Application.Common;
using CemacBourseCalc.Domain.Communs;
using MediatR;

namespace CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteur;

/// <summary>Saisie d'un émetteur (État, entreprise ou organisme supranational de la CEMAC).</summary>
public sealed record AjouterEmetteurCommand(
    string Nom,
    string? Sigle,
    TypeEmetteur Type,
    PaysCemac PaysCode,
    string? SiteWeb) : IRequest<Result<Guid>>;
