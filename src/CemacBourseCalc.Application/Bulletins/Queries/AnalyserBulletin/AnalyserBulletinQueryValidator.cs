using FluentValidation;

namespace CemacBourseCalc.Application.Bulletins.Queries.AnalyserBulletin;

/// <summary>Le fichier doit être un PDF (signature « %PDF- » au début), non vide, de 10 Mo au plus.</summary>
public sealed class AnalyserBulletinQueryValidator : AbstractValidator<AnalyserBulletinQuery>
{
    private static readonly byte[] SignaturePdf = "%PDF-"u8.ToArray();

    public AnalyserBulletinQueryValidator()
    {
        RuleFor(q => q.Contenu)
            .NotEmpty().WithMessage("Choisissez le fichier PDF du bulletin.")
            .Must(c => c.Length <= AnalyserBulletinQuery.TailleMaximum).WithMessage("Le fichier dépasse 10 Mo : ce n'est pas un bulletin de la cote.")
            .Must(c => c.AsSpan().StartsWith(SignaturePdf)).WithMessage("Le fichier n'est pas un PDF.");
    }
}
