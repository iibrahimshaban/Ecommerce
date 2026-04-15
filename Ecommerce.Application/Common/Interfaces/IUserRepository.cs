using Ecommerce.Application.Contracts.Users;
using Ecommerce.Shared.Contracts.Auth;

namespace Ecommerce.Application.Common.Interfaces;
public interface IUserRepository
{
    Task<ApplicationUserDto?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<ApplicationUserDto?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<SignInOutcome> PasswordSignInAsync(string email, string password);
    Task<IEnumerable<string>> GetRoles(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserRolesById(string userId, CancellationToken cancellationToken);
    Task<(string RefreshToken, DateTime RefrechTokenExpiresDate)> GenerateRfreshTokenAsync(int duration,string userId, CancellationToken cancellationToken = default);
    Task<bool> UserEmailExists(string email, string? userId = null, CancellationToken cancellationToken = default);
    Task<bool> UserNameExists(string username, string? userId = null, CancellationToken cancellationToken = default);
    Task<Result<ApplicationUserDto>> CreateUserAsync(ApplicationUserDto user,string password, CancellationToken cancellationToken = default);
    Task<bool> CurrentRefreshTokenAnyAsync(string userId, string refreshToken);
    Task<ConfirmationCodeDto> GenerateConfirmationCodeAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(string email, string newPassowrd, string Code, CancellationToken cancellationToken = default);
    Task<ResetPassowrdDto> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task AddRolesToUserAsync(string email, IList<string> roles, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> UpdateUserAsync(ApplicationUserDto user);
    Task UpdateRolesToUserAsync(string email, IList<string> roles, CancellationToken cancellationToken = default);
    Task<Result> ToggleUserStatusAsync(string userId);
    Task<Result> UnlockUserAsync(string userId);
    Task<UserProfileResponse> GetProfileAsync(string userId);
    Task<Result> UpdateUserProfileAsync(string userId,string username ,string firstName, string lastName);
    Task<Result> ChabgeUserPasswordAsync(string userId, string currentPassword, string newPassword);
    Task<IEnumerable<UserResponse>> GetUsersInRoleAsync(string role,CancellationToken cancellationToken = default);
}
