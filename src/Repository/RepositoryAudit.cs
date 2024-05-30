using Common.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class RepositoryAudit
{
    public static void Audit(RepositoryContext repositoryContext)
    {
        var modifiedEntries = repositoryContext.ChangeTracker.Entries()
            .Where(e => e.State is
                EntityState.Added or
                EntityState.Modified or
                EntityState.Deleted)
            .ToList();

        foreach (var entry in modifiedEntries)
        {
            foreach (var value in entry.OriginalValues.Properties)
            {

                var originalValue = entry.OriginalValues[value];
                var currentValue = entry.CurrentValues[value];

                if (Equals(originalValue, currentValue) && entry.State != EntityState.Added) continue;

                var actionLog = new ActionLog(
                    DateTime.Now,
                    entry.Entity.GetType().Name,
                    (entry.State == EntityState.Added ? -1 : entry.CurrentValues["Id"] ?? -1).ToString() ?? string.Empty,
                    entry.State == EntityState.Modified ? OperationType.Updated : OperationType.Created,
                    value.Name,
                    originalValue?.ToString() ?? string.Empty,
                    currentValue?.ToString() ?? string.Empty);

                repositoryContext.ActionLogs.Add(actionLog);
            }
        }
    }
}