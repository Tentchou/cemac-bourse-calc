using FluentValidation;
using MediatR;

namespace CemacBourseCalc.Application.Common.Behaviors;

/// <summary>Passe chaque requête dans ses validateurs avant le handler ; l'API traduit l'erreur en réponse 400.</summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validateurs)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validateurs.Any()) return await next();

        var contexte = new ValidationContext<TRequest>(request);
        var resultats = await Task.WhenAll(validateurs.Select(v => v.ValidateAsync(contexte, cancellationToken)));
        var erreurs = resultats.SelectMany(r => r.Errors).Where(e => e is not null).ToList();

        if (erreurs.Count > 0) throw new ValidationException(erreurs);
        return await next();
    }
}
