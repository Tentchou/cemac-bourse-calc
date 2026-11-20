using FluentValidation;

namespace CemacBourseCalc.Application.Common.Masse;

/// <summary>Les bornes communes d'un envoi en masse. Chaque ligne est ensuite validée comme une saisie manuelle.</summary>
public static class LignesMasse
{
    public const int Maximum = 500;

    public static IRuleBuilderOptions<T, IReadOnlyList<TLigne>> BornesMasse<T, TLigne>(this IRuleBuilder<T, IReadOnlyList<TLigne>> regle) =>
        regle.NotNull().Must(l => l is { Count: > 0 and <= Maximum })
            .WithMessage($"Envoyez entre 1 et {Maximum} lignes à la fois.");
}
