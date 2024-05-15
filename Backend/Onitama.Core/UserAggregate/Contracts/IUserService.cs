namespace Onitama.Core.Services
{
    public interface IUserService
    {
        Task<IdentityResult> UpdateUserAsync(User user);
        // Define other user-related methods as needed
    }
}