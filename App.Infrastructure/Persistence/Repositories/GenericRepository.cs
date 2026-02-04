using System.Linq.Expressions;
using App.Domain.Common.Interfaces;
using App.Domain.Repositories;
using App.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Persistence.Repositories;

public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> 
    where T : class, IEntityBase
{
    // DbSet'i context üzerinden alıyoruz, constructorda parametre geçmeye gerek yok.
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, bool tracking = true, CancellationToken ct = default)
    {
        
        var entity = await _dbSet.FindAsync([id], cancellationToken: ct);
        
        if (entity is not null && !tracking)
        {
            context.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(bool tracking = true, CancellationToken ct = default)
    {
        return await GetQueryable(tracking).ToListAsync(ct);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool tracking = true, CancellationToken ct = default)
    {
        return await GetQueryable(tracking).FirstOrDefaultAsync(predicate, ct);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        return predicate is null 
            ? await _dbSet.AnyAsync(ct) 
            : await _dbSet.AnyAsync(predicate, ct);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        return predicate is null 
            ? await _dbSet.CountAsync(ct) 
            : await _dbSet.CountAsync(predicate, ct);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, bool tracking = true, CancellationToken ct = default)
    {
        return await GetQueryable(tracking).Where(predicate).ToListAsync(ct);
    }

    public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, bool>>? predicate = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool tracking = true, 
        CancellationToken ct = default)
    {
        var query = GetQueryable(tracking);

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        // Toplam sayıyı filtrelemeden sonra, sayfalama yapmadan önce alıyoruz.
        var totalCount = await query.CountAsync(ct);

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        await _dbSet.AddRangeAsync(entities, ct);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> ExecuteDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        // EF Core 7.0+ ile gelen Bulk Delete özelliği.
        // Veriyi belleğe çekmeden veritabanında doğrudan siler. Çok performanslıdır.
        return await _dbSet.Where(predicate).ExecuteDeleteAsync(ct);
    }

    // --- Helper Method ---
    // Kod tekrarını önlemek için tracking kontrolünü burada yapıyoruz.
    private IQueryable<T> GetQueryable(bool tracking)
    {
        return tracking ? _dbSet.AsQueryable() : _dbSet.AsNoTracking();
    }
}