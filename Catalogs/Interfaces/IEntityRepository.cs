using Catalogs.DB;
using Microsoft.EntityFrameworkCore;

namespace Catalogs.Interfaces
{
    public interface IEntityRepository<TEntity> 
    {
        public Task<List<TEntity>> GetAllAsync();
        public Task AddAsync(TEntity entity);
        public Task DeleteAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
        public Task SaveChangesAsync();
        public Task<TEntity> FindByIdAsync(Guid id);
        public Task<TEntity> FindByIdWithIncludesAsync(Guid id, params string[] includeNames);
        public Task<IEnumerable<TEntity>> Where(Func<TEntity, bool> predicate);
        public Task<List<TEntity>> GetAllAsyncWithIncludes(params string[] includeNames);
        public Task<bool> Any(Func<TEntity, bool> predicate);
        public CatalogDbContext Context { get; }
    }
}
