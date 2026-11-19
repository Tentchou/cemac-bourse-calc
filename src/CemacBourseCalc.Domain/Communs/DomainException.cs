namespace CemacBourseCalc.Domain.Communs;

/// <summary>Une règle métier refusée. Le message est en français et s'affiche tel quel.</summary>
public sealed class DomainException(string message) : Exception(message);
