using DevProject.Domain.Entities;
using DevProject.Domain.Interfaces;
using DevProject.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DevProject.Infrastructure.Persistence.Repositories;

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly AppDbContext Context;

    public Repository(AppDbContext context)
    {
        Context = context;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Context.Set<T>().ToListAsync(cancellationToken);

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        => await Context.Set<T>().CountAsync(cancellationToken);

    public void Create(T entity)
    {
        Context.Add(entity);
    }

    public void Update(T entity)
    {
        Context.Update(entity);
    }

    public void Remove(T entity)
    {
        Context.Remove(entity);
    }
}
