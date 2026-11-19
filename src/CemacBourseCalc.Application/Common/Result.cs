namespace CemacBourseCalc.Application.Common;

/// <summary>Résultat d'une commande : la valeur, ou un message en français à afficher tel quel.</summary>
public sealed record Result<T>(bool Reussi, T? Valeur, string? Message)
{
    public static Result<T> Succes(T valeur, string? message = null) => new(true, valeur, message);
    public static Result<T> Echec(string message) => new(false, default, message);
}
