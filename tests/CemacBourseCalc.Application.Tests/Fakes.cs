using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Application.Common.Interfaces;
using CemacBourseCalc.Application.Place.Interfaces;
using CemacBourseCalc.Domain.Administration;
using CemacBourseCalc.Domain.Place;

namespace CemacBourseCalc.Application.Tests;

/// <summary>Une base en mémoire minimale : ce qui est ajouté n'est « en base » qu'après EnregistrerAsync, et disparaît si la transaction est annulée.</summary>
internal sealed class FausseBase : IUniteDeTravail
{
    private readonly List<Action> _enAttente = [];
    private readonly List<Action> _annulations = [];

    public HashSet<DateOnly> JoursEnregistres { get; } = [];
    public List<Administrateur> Administrateurs { get; } = [];
    public List<JetonSession> Sessions { get; } = [];
    public List<(Guid Auteur, string Action, string Entite, string EntiteId)> Journal { get; } = [];

    public bool TransactionValidee { get; private set; }
    public bool TransactionAnnulee { get; private set; }
    private bool _enTransaction;

    public void Plus_tard(Action enregistrer, Action annuler)
    {
        _enAttente.Add(enregistrer);
        _annulations.Add(annuler);
    }

    public Task EnregistrerAsync(CancellationToken ct)
    {
        foreach (var a in _enAttente) a();
        _enAttente.Clear();
        if (!_enTransaction) _annulations.Clear();
        return Task.CompletedTask;
    }

    public Task<ITransactionEcriture> CommencerTransactionAsync(CancellationToken ct)
    {
        _enTransaction = true;
        return Task.FromResult<ITransactionEcriture>(new Transaction(this));
    }

    private sealed class Transaction(FausseBase base_) : ITransactionEcriture
    {
        public Task ValiderAsync(CancellationToken ct)
        {
            base_.TransactionValidee = true;
            base_._annulations.Clear();
            base_._enTransaction = false;
            return Task.CompletedTask;
        }

        public Task AnnulerAsync(CancellationToken ct)
        {
            base_.TransactionAnnulee = true;
            foreach (var a in base_._annulations) a();
            base_._annulations.Clear();
            base_._enTransaction = false;
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

internal sealed class FauxJoursNonOuvres(FausseBase base_) : IJoursNonOuvresEcriture
{
    public Task<bool> ExisteAsync(DateOnly date, CancellationToken ct) => Task.FromResult(base_.JoursEnregistres.Contains(date));

    public void Ajouter(JourNonOuvre jour) =>
        base_.Plus_tard(() => base_.JoursEnregistres.Add(jour.Date), () => base_.JoursEnregistres.Remove(jour.Date));
}

internal sealed class FauxJournal(FausseBase base_) : IJournalAdmin
{
    public void Noter(Guid auteurId, string action, string entite, string entiteId, object? avant = null, object? apres = null)
    {
        var ligne = (auteurId, action, entite, entiteId);
        base_.Plus_tard(() => base_.Journal.Add(ligne), () => base_.Journal.Remove(ligne));
    }
}

internal sealed class FauxAdministrateurs(FausseBase base_) : IAdministrateursEcriture
{
    public Task<Administrateur?> ParEmailAsync(string emailMinuscules, CancellationToken ct) =>
        Task.FromResult(base_.Administrateurs.SingleOrDefault(a => a.Email == emailMinuscules));

    public Task<bool> EmailExisteAsync(string emailMinuscules, CancellationToken ct) =>
        Task.FromResult(base_.Administrateurs.Any(a => a.Email == emailMinuscules));

    public void Ajouter(Administrateur administrateur) =>
        base_.Plus_tard(() => base_.Administrateurs.Add(administrateur), () => base_.Administrateurs.Remove(administrateur));

    public Task<JetonSession?> SessionParEmpreinteAsync(string empreinte, CancellationToken ct) =>
        Task.FromResult(base_.Sessions.SingleOrDefault(s => s.JetonHash == empreinte));

    public void AjouterSession(JetonSession session) =>
        base_.Plus_tard(() => base_.Sessions.Add(session), () => base_.Sessions.Remove(session));
}

internal sealed class FauxAdministrateurCourant(Guid? id) : IAdministrateurCourant
{
    public Guid? Id { get; } = id;
}
