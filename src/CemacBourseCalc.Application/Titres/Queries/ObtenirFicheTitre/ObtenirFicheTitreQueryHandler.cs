using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Conventions;
using MediatR;

namespace CemacBourseCalc.Application.Titres.Queries.ObtenirFicheTitre;

public sealed class ObtenirFicheTitreQueryHandler(ITitresLecture lecture)
    : IRequestHandler<ObtenirFicheTitreQuery, FicheTitreDto?>
{
    public async Task<FicheTitreDto?> Handle(ObtenirFicheTitreQuery request, CancellationToken cancellationToken)
    {
        var fiche = await lecture.FicheAsync(request.Id, cancellationToken);
        if (fiche?.Obligation is not { } obligation) return fiche;

        // Le code stocké en base devient le libellé lisible du Domain (« EXACT_365 » → « Exact/365 »).
        var libelle = ConventionsDecompte.Trouver(obligation.Convention)?.Libelle ?? obligation.Convention;
        return fiche with { Obligation = obligation with { Convention = libelle } };
    }
}
