using CemacBourseCalc.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace CemacBourseCalc.Infrastructure.Persistence;

public sealed class UniteDeTravail(CalcDbContext db) : IUniteDeTravail
{
    public Task EnregistrerAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<ITransactionEcriture> CommencerTransactionAsync(CancellationToken ct) =>
        new TransactionEcriture(await db.Database.BeginTransactionAsync(ct));

    /// <summary>Libérée sans validation (erreur, exception), la transaction EF est annulée par PostgreSQL.</summary>
    private sealed class TransactionEcriture(IDbContextTransaction transaction) : ITransactionEcriture
    {
        public Task ValiderAsync(CancellationToken ct) => transaction.CommitAsync(ct);
        public Task AnnulerAsync(CancellationToken ct) => transaction.RollbackAsync(ct);
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
