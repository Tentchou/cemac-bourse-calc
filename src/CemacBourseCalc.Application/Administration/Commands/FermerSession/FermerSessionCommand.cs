using CemacBourseCalc.Application.Common;
using MediatR;

namespace CemacBourseCalc.Application.Administration.Commands.FermerSession;

/// <summary>Déconnexion : la session est révoquée en base, le jeton ne sert plus, même copié ailleurs.</summary>
public sealed record FermerSessionCommand(string Jeton) : IRequest<Result<bool>>
{
    public override string ToString() => nameof(FermerSessionCommand);
}
