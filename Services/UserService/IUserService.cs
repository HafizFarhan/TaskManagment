using Injazat.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Task = System.Threading.Tasks.Task;

namespace Injazat.Presentation.Services.UserService
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(User user, string password, string roleName);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<SignInResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignOutAsync();
        Task<bool> IsEmailConfirmedAsync(User user);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);

        // Role Management Methods
        Task<IdentityResult> AddUserToRoleAsync(User user, string roleName);
        Task<IdentityResult> RemoveUserFromRoleAsync(User user, string roleName);
        Task<bool> IsUserInRoleAsync(User user, string roleName);
        Task<IList<string>> GetUserRolesAsync(User user);
    }
}
