using Microsoft.AspNetCore.Identity;
using Onitama.Core.UserAggregate;
using Onitama.Core.Services;
using System.Threading.Tasks;

namespace Onitama.Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        // Implement other methods as needed
    }
}