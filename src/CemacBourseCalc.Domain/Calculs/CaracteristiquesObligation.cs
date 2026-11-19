using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Conventions;

namespace CemacBourseCalc.Domain.Calculs;

/// <summary>
/// Ce dont les calculs ont besoin d'une obligation, sans rien d'autre. Les calculs lisent ce
/// record et jamais la base : on peut les tester avec des valeurs écrites à la main.
/// </summary>
public sealed record CaracteristiquesObligation(
    string Nom,
    decimal Nominal,
    decimal TauxFacial,
    Periodicite Periodicite,
    DateOnly DateJouissance,
    DateOnly? DatePremierCoupon,
    DateOnly DateEcheance,
    ModeRemboursement ModeRemboursement,
    int DiffereEcheances,
    string CodeConvention)
{
    public IConventionDecompte Convention =>
        ConventionsDecompte.Trouver(CodeConvention)
        ?? throw new DomainException($"La convention « {CodeConvention} » du titre {Nom} n'est pas connue.");
}
