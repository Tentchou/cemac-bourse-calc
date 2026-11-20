using CemacBourseCalc.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace CemacBourseCalc.Application.Common.Masse;

/// <summary>
/// La saisie en masse n'a aucune règle à elle : chaque ligne est envoyée telle quelle à la commande
/// de saisie manuelle (mêmes validations, mêmes contrôles, même journal), dans une seule transaction.
///
/// - Les doublons à l'intérieur de l'envoi sont signalés avec le numéro de la première ligne.
/// - Toutes les lignes sont essayées, pour rendre toutes les erreurs en une fois.
/// - À la moindre erreur, la transaction est annulée : rien n'est enregistré.
/// </summary>
public sealed class ExecuteurEnMasse(ISender mediator, IUniteDeTravail uniteDeTravail)
{
    /// <param name="cle">Ce qui doit être unique dans l'envoi (ISIN, date…), ou null si les doublons sont permis.</param>
    /// <param name="libelleCle">Le nom de cette clé dans le message : « le même ISIN », « la même date »…</param>
    public async Task<Result<ResultatMasseDto>> ExecuterAsync<TCommande, TId>(
        IReadOnlyList<TCommande> lignes, Func<TCommande, string?>? cle, string libelleCle, CancellationToken ct)
        where TCommande : IRequest<Result<TId>>
    {
        var erreurs = new List<ErreurLigneDto>();
        var premiereLigneParCle = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        await using var transaction = await uniteDeTravail.CommencerTransactionAsync(ct);

        for (var i = 0; i < lignes.Count; i++)
        {
            var numero = i + 1;
            var ligne = lignes[i];

            if (ligne is null)
            {
                erreurs.Add(new ErreurLigneDto(numero, ["La ligne est vide."]));
                continue;
            }

            if (cle?.Invoke(ligne)?.Trim() is { Length: > 0 } valeur)
            {
                if (premiereLigneParCle.TryGetValue(valeur, out var premiere))
                {
                    erreurs.Add(new ErreurLigneDto(numero, [$"{libelleCle} que la ligne {premiere}."]));
                    continue;
                }
                premiereLigneParCle[valeur] = numero;
            }

            try
            {
                var resultat = await mediator.Send(ligne, ct);
                if (!resultat.Reussi) erreurs.Add(new ErreurLigneDto(numero, [resultat.Message ?? "Ligne refusée."]));
            }
            catch (ValidationException v)
            {
                erreurs.Add(new ErreurLigneDto(numero, v.Errors.Select(e => e.ErrorMessage).Distinct().ToList()));
            }
        }

        if (erreurs.Count > 0)
        {
            await transaction.AnnulerAsync(ct);
            return Result<ResultatMasseDto>.Succes(new ResultatMasseDto(lignes.Count, 0, erreurs),
                $"Rien n'a été enregistré : {erreurs.Count} ligne(s) à corriger sur {lignes.Count}.");
        }

        await transaction.ValiderAsync(ct);
        return Result<ResultatMasseDto>.Succes(new ResultatMasseDto(lignes.Count, lignes.Count, []),
            $"{lignes.Count} ligne(s) enregistrée(s).");
    }
}
