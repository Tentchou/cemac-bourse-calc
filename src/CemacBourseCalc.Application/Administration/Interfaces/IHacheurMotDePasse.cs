namespace CemacBourseCalc.Application.Administration.Interfaces;

public enum VerificationMotDePasse
{
    Incorrect,
    Correct,
    /// <summary>Correct, mais l'empreinte utilise des paramètres dépassés : la recalculer.</summary>
    CorrectARehacher
}

/// <summary>Empreinte lente et salée des mots de passe (PBKDF2). Le mot de passe en clair n'est jamais conservé.</summary>
public interface IHacheurMotDePasse
{
    string Hacher(string motDePasse);
    VerificationMotDePasse Verifier(string empreinte, string motDePasse);

    /// <summary>Même coût qu'une vraie vérification, pour qu'un e-mail inconnu ne réponde pas plus vite.</summary>
    void VerifierFactice(string motDePasse);
}
