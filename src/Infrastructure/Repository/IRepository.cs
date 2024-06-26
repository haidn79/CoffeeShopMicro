using Infrastructure.Domain;
using Infrastructure.Specification;

namespace Infrastructure.Repository;

public interface IRepository<TEntity> where TEntity : IAggregateRoot
{
    Task<TEntity> FindById(Guid id, CancellationToken cancellationToken = default);
    Task<TEntity> FindOneAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<List<TEntity>> FindAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);
    Task<TEntity> EditAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);
}