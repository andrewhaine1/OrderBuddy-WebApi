using Microsoft.EntityFrameworkCore;
using Ord.WebApi.Data.Contexts;
using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.Shared
{
    public class DataRepository<T> : IDataRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public DbSet<T> EntityDbSet { get => _context.Set<T>(); }

        public DataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public DbSet<T> GetDbSet()
            => _context.Set<T>();

        public async Task<IEnumerable<T>> GetEntitesListAsync()
            => await _context.Set<T>().ToListAsync();

        public async Task<PagedList<T>> GetEntitesPagedListAsync(ResourceParameters resourceParameters)
            => await PagedList<T>.CreateAsync(_context.Set<T>(),
                resourceParameters.PageNumber,
                resourceParameters.PageSize);

        public async Task<PagedList<T>> GetEntitesPagedListAsync(int entityId, 
            ResourceParameters resourceParameters)
            => await PagedList<T>.CreateAsync(_context.Set<T>()
                .Where(e => e.Id == entityId),
                resourceParameters.PageNumber,
                resourceParameters.PageSize);

        public async Task<T> GetEntityAsync(int entityId)
            => await _context.Set<T>().Where(e => e.Id == entityId)
            .FirstOrDefaultAsync();

        public void AddEntity(T entity)
            => _context.Add(entity);

        public void DeleteEntity(T entity)
            => _context.Remove(entity);

        public void UpdateEntity(T entity)
            => _context.Update(entity);

        public bool SaveChanges()
            => _context.SaveChanges() >= 0;

        public async Task<bool> SaveChangesAsync()
            => await _context.SaveChangesAsync() >= 0;
    }
}
