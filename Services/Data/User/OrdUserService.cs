using Microsoft.EntityFrameworkCore;
using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Paging;
using Ord.WebApi.Services.Data.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace Ord.WebApi.Services.Data.User
{
    public class OrdUserService : IOrdUserService
    {
        private readonly IDataRepository<OrdUser> _userRepo;

        public OrdUserService(IDataRepository<OrdUser> userRepo)
        {
            _userRepo = userRepo;
        }

        // Get all orders for user.
        public async Task<PagedList<OrdUser>> GetOrdUsersAsync(
            ResourceParameters resourceParameters)
            => await PagedList<OrdUser>.CreateAsync(_userRepo.EntityDbSet,
                resourceParameters.PageNumber,
                resourceParameters.PageSize);

        public async Task<OrdUser> GetOrdUserAsync(int userId)
           => await _userRepo.GetEntityAsync(userId);

        public async Task<OrdUser> GetOrdUserByOAuthIdAsync(int oauthId)
            => await _userRepo.EntityDbSet
            .Where(u => u.OauthId == oauthId)
            .FirstOrDefaultAsync();

        public void CreateUser(OrdUser ordUser)
            => _userRepo.AddEntity(ordUser);

        public void UpdateUser(OrdUser ordUser)
            => _userRepo.UpdateEntity(ordUser);

        public void DeleteUser(OrdUser ordUser)
            => _userRepo.DeleteEntity(ordUser);

        public async Task<bool> SaveChangesAsync()
            => await _userRepo.SaveChangesAsync();
    }
}
