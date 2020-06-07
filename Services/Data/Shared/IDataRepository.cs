using Microsoft.EntityFrameworkCore;
using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.Shared
{
    public interface IDataRepository<T> where T : BaseEntity
    {
        DbSet<T> GetDbSet();

        DbSet<T> EntityDbSet { get; }

        Task<PagedList<T>> GetEntitesPagedListAsync(ResourceParameters resourceParameters);

        Task<PagedList<T>> GetEntitesPagedListAsync(int id, ResourceParameters resourceParameters);

        Task<IEnumerable<T>> GetEntitesListAsync();

        Task<T> GetEntityAsync(int id);

        void AddEntity(T entity);

        void UpdateEntity(T entity);

        void DeleteEntity(T entity);

        bool SaveChanges();

        Task<bool> SaveChangesAsync();
    }
}
