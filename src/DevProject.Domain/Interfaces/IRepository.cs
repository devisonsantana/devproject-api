using DevProject.Domain.Entities;

namespace DevProject.Domain.Interfaces;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}
