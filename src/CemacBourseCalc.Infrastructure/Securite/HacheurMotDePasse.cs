using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Domain.Administration;
using Microsoft.AspNetCore.Identity;

namespace CemacBourseCalc.Infrastructure.Securite;

/// <summary>
/// L'empreinte d'ASP.NET Core Identity (PBKDF2-HMAC-SHA512, 100 000 itérations, sel aléatoire), sans le
/// reste d'Identity. Format versionné : si Microsoft durcit les paramètres, l'empreinte est recalculée
/// à la connexion suivante (<see cref="VerificationMotDePasse.CorrectARehacher"/>).
/// </summary>
public sealed class HacheurMotDePasse : IHacheurMotDePasse
{
    private static readonly PasswordHasher<Administrateur> Hacheur = new();

    // Empreinte d'un mot de passe jetable, pour donner le même temps de réponse à un e-mail inconnu.
    private static readonly Lazy<string> EmpreinteFactice = new(() => Hacheur.HashPassword(null!, Guid.NewGuid().ToString()));

    public string Hacher(string motDePasse) => Hacheur.HashPassword(null!, motDePasse);

    public VerificationMotDePasse Verifier(string empreinte, string motDePasse) =>
        Hacheur.VerifyHashedPassword(null!, empreinte, motDePasse) switch
        {
            PasswordVerificationResult.Success => VerificationMotDePasse.Correct,
            PasswordVerificationResult.SuccessRehashNeeded => VerificationMotDePasse.CorrectARehacher,
            _ => VerificationMotDePasse.Incorrect
        };

    public void VerifierFactice(string motDePasse) => Hacheur.VerifyHashedPassword(null!, EmpreinteFactice.Value, motDePasse);
}
