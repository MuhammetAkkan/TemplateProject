using System.Linq.Expressions;
using App.Domain.Common.Interfaces;

namespace App.Domain.Repositories;

public interface IGenericRepository<T> where T : class, IEntityBase
{
    
    // QUERY (Okuma)
    Task<T?> GetByIdAsync(Guid id, bool tracking = true, CancellationToken ct = default);
    
    Task<IReadOnlyList<T>> GetAllAsync(bool tracking = true, CancellationToken ct = default);
    
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool tracking = true, CancellationToken ct = default);
    
    Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
    
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

    // Listeleme ve Include aynı anda yönetilebilir
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, bool tracking = true, CancellationToken ct = default);

    // 3. PAGINATION: Dönüş tipi tuple yerine özel bir Paginate modeli olabilir ama şimdilik bu yapı da olur.
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, // Expression yerine Func daha esnektir
        bool tracking = true,
        CancellationToken ct = default);


    // Command (Yazma)
    Task AddAsync(T entity, CancellationToken ct = default);
    
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
    
    // Update ve Delete senkron kalabilir, EF Core sadece State değiştirir.
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);

    // Belirli bir şarta uyan kayıtları silme
    Task<int> ExecuteDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
}