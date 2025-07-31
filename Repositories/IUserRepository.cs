using TasteTrack_RMS.Models;

namespace TasteTrack_RMS.Repositories
{
    public interface IUserRepository
    {
        Task<LoginResponse> AuthenticateAsync(string userid, string password);
        Task<usermaster?> GetUserByIdAsync(string userid);
        Task<List<usermaster>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(usermaster user);
        Task<bool> UpdateUserAsync(usermaster user);
        Task<bool> DeleteUserAsync(string userid);
        Task<bool> UserExistsAsync(string userid);
    }
}
