using System.Text.Json;
using System.Text.Json.Serialization;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Domain.Administration;
using CemacBourseCalc.Infrastructure.Persistence;

namespace CemacBourseCalc.Infrastructure.Ecriture;

public sealed class JournalAdminEcriture(CalcDbContext db, TimeProvider horloge) : IJournalAdmin
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public void Noter(Guid auteurId, string action, string entite, string entiteId, object? avant = null, object? apres = null) =>
        db.JournalAdmin.Add(new JournalAdmin(auteurId, horloge.GetUtcNow(), action, entite, entiteId, EnJson(avant), EnJson(apres)));

    private static string? EnJson(object? valeur) => valeur is null ? null : JsonSerializer.Serialize(valeur, valeur.GetType(), Options);
}
