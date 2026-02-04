using App.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace App.Infrastructure.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        // Sadece IEntityBase arayüzünü uygulayanları yakalar
        foreach (var entry in context.ChangeTracker.Entries<IEntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                // Eğer constructor'da atanmamışsa garanti altına al
                if (entry.Entity.CreatedAt == default)
                {
                    entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }
}