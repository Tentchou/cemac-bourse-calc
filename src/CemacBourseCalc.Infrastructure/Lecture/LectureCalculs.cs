using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>
/// Lectures des calculs avec Dapper. Requêtes paramétrées uniquement (jamais de concaténation).
/// Les dates passent en DateTime puis sont converties : Dapper ne connaît pas DateOnly.
/// </summary>
public sealed class LectureCalculs(NpgsqlDataSource source) : ILectureCalculs
{
    private const string SqlObligation = """
        SELECT t.nom                 AS Nom,
               t.nominal             AS Nominal,
               o.taux_facial         AS TauxFacial,
               o.periodicite         AS Periodicite,
               o.date_jouissance     AS DateJouissance,
               o.date_premier_coupon AS DatePremierCoupon,
               o.date_echeance       AS DateEcheance,
               o.mode_remboursement  AS ModeRemboursement,
               o.differe_echeances   AS DiffereEcheances,
               o.code_convention     AS CodeConvention
        FROM titre t
        JOIN obligation o ON o.titre_id = t.id
        WHERE t.id = @TitreId
        """;

    private const string SqlParametre = "SELECT valeur FROM parametre WHERE cle = @Cle";

    private const string SqlJoursNonOuvres = "SELECT date FROM jour_non_ouvre WHERE date BETWEEN @Du::date AND @Au::date";

    public async Task<CaracteristiquesObligation?> ObligationAsync(Guid titreId, CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var ligne = await connexion.QuerySingleOrDefaultAsync<LigneObligation>(
            new CommandDefinition(SqlObligation, new { TitreId = titreId }, cancellationToken: ct));

        return ligne is null ? null : new CaracteristiquesObligation(
            ligne.Nom, ligne.Nominal, ligne.TauxFacial,
            Enum.Parse<Periodicite>(ligne.Periodicite),
            DateOnly.FromDateTime(ligne.DateJouissance),
            ligne.DatePremierCoupon is { } p ? DateOnly.FromDateTime(p) : null,
            DateOnly.FromDateTime(ligne.DateEcheance),
            Enum.Parse<ModeRemboursement>(ligne.ModeRemboursement),
            ligne.DiffereEcheances,
            ligne.CodeConvention);
    }

    public async Task<string?> ParametreAsync(string cle, CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        return await connexion.QuerySingleOrDefaultAsync<string>(new CommandDefinition(SqlParametre, new { Cle = cle }, cancellationToken: ct));
    }

    public async Task<IReadOnlySet<DateOnly>> JoursNonOuvresAsync(DateOnly du, DateOnly au, CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var dates = await connexion.QueryAsync<DateTime>(new CommandDefinition(SqlJoursNonOuvres,
            new { Du = du.ToDateTime(TimeOnly.MinValue), Au = au.ToDateTime(TimeOnly.MinValue) }, cancellationToken: ct));
        return dates.Select(DateOnly.FromDateTime).ToHashSet();
    }

    private sealed class LigneObligation
    {
        public string Nom { get; init; } = null!;
        public decimal Nominal { get; init; }
        public decimal TauxFacial { get; init; }
        public string Periodicite { get; init; } = null!;
        public DateTime DateJouissance { get; init; }
        public DateTime? DatePremierCoupon { get; init; }
        public DateTime DateEcheance { get; init; }
        public string ModeRemboursement { get; init; } = null!;
        public int DiffereEcheances { get; init; }
        public string CodeConvention { get; init; } = null!;
    }
}
