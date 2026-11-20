using CemacBourseCalc.Application.Administration.Dtos;
using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Commands.OuvrirSession;

/// <summary>Connexion d'un administrateur par e-mail et mot de passe.</summary>
public sealed record OuvrirSessionCommand(string Email, string MotDePasse) : IRequest<Result<SessionOuverteDto>>
{
    // Le mot de passe ne doit jamais apparaître dans un journal ou une trace.
    public override string ToString() => $"{nameof(OuvrirSessionCommand)} {{ Email = {Email} }}";
}
