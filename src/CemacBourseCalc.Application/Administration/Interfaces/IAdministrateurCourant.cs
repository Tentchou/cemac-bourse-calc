namespace CemacBourseCalc.Application.Administration.Interfaces;

/// <summary>L'administrateur de la session en cours, ou null hors session (ligne de commande, requête publique).</summary>
public interface IAdministrateurCourant
{
    Guid? Id { get; }
}
