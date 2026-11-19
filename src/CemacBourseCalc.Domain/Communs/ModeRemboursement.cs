namespace CemacBourseCalc.Domain.Communs;

public enum ModeRemboursement
{
    /// <summary>Tout le capital est remboursé à l'échéance.</summary>
    InFine,

    /// <summary>Le capital est remboursé par parts égales à chaque échéance, après l'éventuel différé.</summary>
    AmortissementConstant
}
