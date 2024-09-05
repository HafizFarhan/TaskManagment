using Microsoft.AspNetCore.Identity;
using Injazat.DataAccess.Models;
using Injazat.Presentation.Services.LogDBService;
using Task = System.Threading.Tasks.Task;

namespace Injazat.Presentation.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserService> _logger;
        private readonly ILogDbService _logDbService;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<UserService> logger, ILogDbService logDbService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _logDbService = logDbService;
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password, string roleName)
        {
            user.CreationDate = DateTime.UtcNow;
            user.ModificationDate = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Assign the role to the user
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"User '{user.Email}' created and assigned role '{roleName}'.");
                    _logDbService.AddLogEvent(new LogEvent
                    {
                        Action = "Create User",
                        Description = $"User '{user.Email}' created and assigned role '{roleName}'.",
                    });
                }
                else
                {
                    _logger.LogWarning($"User '{user.Email}' created, but role '{roleName}' assignment failed.");
                    _logDbService.AddLogEvent(new LogEvent
                    {
                        Action = "Create User",
                        Description = $"User '{user.Email}' created, but role '{roleName}' assignment failed.",
                    });
                    return roleResult; // Return role assignment failure if any
                }
            }
            else
            {
                _logger.LogError($"Failed to create user '{user.Email}'.");
            }

            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            user.ModificationDate = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User updated successfully: {user.Email}");
                _logDbService.AddLogEvent(new LogEvent
                {
                    Action = "Update User",
                    Description = $"User {user.Email} updated successfully.",
                    UserId = user.Id,
                    User = user,
                    UserName = user.UserName!
                });
            }
            return result;
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.ModificationDate = DateTime.UtcNow;
                user.IsDeleted = true;
                user.DeletionDate = DateTime.UtcNow;

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User deleted successfully: {user.Email}");
                    _logDbService.AddLogEvent(new LogEvent
                    {
                        Action = "Delete User",
                        Description = $"User {user.Email} deleted successfully.",
                        UserId = user.Id,
                        User = user,
                        UserName = user.UserName!
                    });
                }
                return result;
            }
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<SignInResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User signed out.");
        }

        public async Task<bool> IsEmailConfirmedAsync(User user)
        {
            var isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            _logger.LogInformation($"Email confirmation status for {user.Email}: {isConfirmed}");
            return isConfirmed;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation($"Generated password reset token for {user.Email}");
            _logDbService.AddLogEvent(new LogEvent
            {
                Action = "Generate Password Reset Token",
                Description = $"Password reset token generated for user {user.Email}.",
                UserId = user.Id,
                User = user,
                UserName = user.UserName!
            });
            return token;
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Password reset successfully for {user.Email}");
                _logDbService.AddLogEvent(new LogEvent
                {
                    Action = "Reset Password",
                    Description = $"Password reset successfully for user {user.Email}.",
                    UserId = user.Id,
                    User = user,
                    UserName = user.UserName!
                });
            }
            else
            {
                _logger.LogWarning($"Failed to reset password for {user.Email}");
                _logDbService.AddLogEvent(new LogEvent
                {
                    Action = "Failed Password Reset",
                    Description = $"Failed to reset password for user {user.Email}.",
                    UserId = user.Id,
                    User = user,
                    UserName = user.UserName!
                });
            }
            return result;
        }

        // Role Management Methods

        public async Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User '{user.Email}' added to role '{roleName}'.");
                _logDbService.AddLogEvent(new LogEvent
                {
                    Action = "Add User To Role",
                    Description = $"User '{user.Email}' added to role '{roleName}'.",
                    UserId = user.Id,
                    User = user,
                    UserName = user.UserName!
                });
            }
            else
            {
                _logger.LogWarning($"Failed to add user '{user.Email}' to role '{roleName}'.");
            }
            return result;
        }

        public async Task<IdentityResult> RemoveUserFromRoleAsync(User user, string roleName)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User '{user.Email}' removed from role '{roleName}'.");
                _logDbService.AddLogEvent(new LogEvent
                {
                    Action = "Remove User From Role",
                    Description = $"User '{user.Email}' removed from role '{roleName}'.",
                    UserId = user.Id,
                    User = user,
                    UserName = user.UserName!
                });
            }
            else
            {
                _logger.LogWarning($"Failed to remove user '{user.Email}' from role '{roleName}'.");
            }
            return result;
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            var isInRole = await _userManager.IsInRoleAsync(user, roleName);
            _logger.LogInformation($"Check if user '{user.Email}' is in role '{roleName}': {isInRole}");
            return isInRole;
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation($"Retrieved roles for user '{user.Email}': {string.Join(", ", roles)}");
            return roles;
        }
    }
}
