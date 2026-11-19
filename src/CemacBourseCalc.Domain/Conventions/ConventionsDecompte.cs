namespace CemacBourseCalc.Domain.Conventions;

/// <summary>Les conventions connues, retrouvées par leur code.</summary>
public static class ConventionsDecompte
{
    public static readonly IConventionDecompte Exact365 = new ConventionExact365();
    public static readonly IConventionDecompte Exact360 = new ConventionExact360();
    public static readonly IConventionDecompte Trente360Europeen = new Convention30E360();

    public static IReadOnlyList<IConventionDecompte> Toutes { get; } = [Exact365, Exact360, Trente360Europeen];

    public static IConventionDecompte? Trouver(string? code) =>
        Toutes.FirstOrDefault(c => string.Equals(c.Code, code?.Trim(), StringComparison.OrdinalIgnoreCase));
}
