using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.User
{
    public interface IOrdUserService
    {
        /* ------------------------------------------ Users ----------------------------------------- */
        Task<PagedList<OrdUser>> GetOrdUsersAsync(ResourceParameters resourceParameters);

        Task<OrdUser> GetOrdUserAsync(int userId);

        Task<OrdUser> GetOrdUserByOAuthIdAsync(int oauthId);

        void CreateUser(OrdUser ordUser);

        void UpdateUser(OrdUser ordUser);

        void DeleteUser(OrdUser ordUser);

        Task<bool> SaveChangesAsync();
    }
}
