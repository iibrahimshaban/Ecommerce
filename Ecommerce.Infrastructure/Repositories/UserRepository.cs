using Ecommerce.Application.Common.Results;
using Ecommerce.Application.Contracts.Users;
using Ecommerce.Application.Errors;
using Ecommerce.Core.Errors;
using Ecommerce.Shared.Authorization;
using Ecommerce.Shared.Contracts.Auth;
using Ecommerce.Shared.Contracts.Users;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Ecommerce.Infrastructure.Repositories;
public class UserRepository(
    UserManager<ApplicationUser> userManager, 
    SignInManager<ApplicationUser> signInManager ,
    ApplicationDbContext context ,
    ILogger<UserRepository> logger) : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<UserRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task<ApplicationUserDto?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        return user.Adapt<ApplicationUserDto>();
    }
    public async Task<ApplicationUserDto?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        return user.Adapt<ApplicationUserDto>();
    }
    public async Task<SignInOutcome> PasswordSignInAsync(string userName, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(userName, password, false, true);

        return new SignInOutcome(
        result.Succeeded,
        result.IsLockedOut,
        result.RequiresTwoFactor,
        result.IsNotAllowed
        );
    }
    public async Task<IEnumerable<string>> GetRoles(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
                        .Where(x => !x.IsDeleted)
                        .Select(x => x.Name.ToString())
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

    }
    public async Task<IEnumerable<string>> GetUserRolesById(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return [];
        var userRoles = await _userManager.GetRolesAsync(user);

        return userRoles;
    }

    public async Task<(string RefreshToken, DateTime RefrechTokenExpiresDate)> GenerateRfreshTokenAsync(int duration, string userId, CancellationToken cancellationToken = default)
    {
        var RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var user = await _userManager.FindByIdAsync(userId);

        var RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(duration);

        user!.RefreshTokens.Add(new RefreshToken
        {
            Token = RefreshToken,
            ExpiresOn = RefreshTokenExpiryDate,
        });

        await _userManager.UpdateAsync(user);

        return (RefreshToken, RefreshTokenExpiryDate);
    }

    public  async Task<bool> UserEmailExists(string email,string? userId = null, CancellationToken cancellationToken = default)
    {
        return await _userManager.Users
            .AnyAsync(x => x.Email == email && (userId == null || x.Id != userId), cancellationToken);

    }
    public async Task<bool> UserNameExists(string username,string? userId = null, CancellationToken cancellationToken = default)
    {
        return await _userManager.Users
        .AnyAsync(x => x.UserName == username && (userId == null || x.Id != userId), cancellationToken);
    }

    public async Task<Result<ApplicationUserDto>> CreateUserAsync(ApplicationUserDto userDto, string password , CancellationToken cancellationToken = default)
    {
        var User = new ApplicationUser
        {
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            UserName = userDto.UserName,
            IsDisabled = userDto.IsDisabled,
            LockoutEnd = userDto.LockoutEnd,
            EmailConfirmed = userDto.EmailConfirmed
        };

        var result = await _userManager.CreateAsync(User, password);

        if (result.Succeeded)
        {
            return Result.Success(User.Adapt<ApplicationUserDto>());
        }     

        var error = result.Errors.FirstOrDefault();

        return Result.Failure<ApplicationUserDto>(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<bool> CurrentRefreshTokenAnyAsync(string userId, string refreshToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var UserRefreshToken = user!.RefreshTokens
            .SingleOrDefault(x => x.Token == refreshToken && x.IsActivated);

        if (UserRefreshToken is null )
            return false;
        else
        {
            UserRefreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }
    }

    public async Task<ConfirmationCodeDto> GenerateConfirmationCodeAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user!);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


        _logger.LogInformation("Verification code : {code}", code);

        return new ConfirmationCodeDto(user.Adapt<ApplicationUserDto>() ,code , user.Id);
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken = default)
    {
        var User = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ConfirmEmailAsync(User!,code);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(User, DefaultRoles.Member.Name);
            return Result.Success();
        }

        var error = result.Errors.FirstOrDefault();

        return Result.Failure(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> ResetPasswordAsync(string email,string newPassowrd,string Code ,CancellationToken cancellationToken = default)
    {
        var user =  await _userManager.FindByEmailAsync(email);
        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
            result = await _userManager.ResetPasswordAsync(user!, code, newPassowrd);
        }
        catch
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }

    public async Task<ResetPassowrdDto> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        var code = await _userManager.GeneratePasswordResetTokenAsync(user!);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("reset code : {code}", code);

        return new ResetPassowrdDto(user.Adapt<ApplicationUserDto>() , code , user.Id);
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var allUsers = await(from user in _context.Users
                             join userRole in _context.UserRoles
                             on user.Id equals userRole.UserId
                             join role in _context.Roles
                             on userRole.RoleId equals role.Id into roles
                             where roles.All(x => x.Name != DefaultRoles.Member.Name)
                             select new
                             {
                                 user.Id,
                                 user.UserName,
                                 user.FirstName,
                                 user.LastName,
                                 user.Email,
                                 user.IsDisabled,
                                 roles = roles.Select(x => x.Name!).ToList()
                             }
                             )
                             .GroupBy(u => new { u.Id, u.UserName, u.FirstName, u.LastName, u.Email, u.IsDisabled })
                             .Select(x => new UserResponse
                             (
                                x.Key.Id,
                                x.Key.UserName!,
                                x.Key.FirstName,
                                x.Key.LastName,
                                x.Key.Email,
                                x.Key.IsDisabled,
                                x.SelectMany(r => r.roles)
                             ))
                            .ToListAsync(cancellationToken);

        return allUsers;
    }

    public async Task AddRolesToUserAsync(string email, IList<string> roles, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        await _userManager.AddToRolesAsync(user, roles);

    }
    public async Task DeleteUserAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        await _userManager.DeleteAsync(user);
    }
    public async Task<Result> UpdateUserAsync(ApplicationUserDto user)
    {
        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        var config = new TypeAdapterConfig();
        config.NewConfig<ApplicationUserDto, ApplicationUser>()
              .Ignore(dest => dest.Id);

        existingUser = user.Adapt(existingUser, config);
        var result = await _userManager.UpdateAsync(existingUser);

        if (result.Succeeded) 
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task UpdateRolesToUserAsync(string email, IList<string> roles, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        await _context.UserRoles
                .Where(x => x.UserId == user.Id)
                .ExecuteDeleteAsync(cancellationToken);

        await _userManager.AddToRolesAsync(user,roles);
    }

    public async Task<Result> ToggleUserStatusAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user.IsDisabled = !user.IsDisabled;
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }
    public async Task<Result> UnlockUserAsync(string userId)
    {
        if(await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow);

        return Result.Success();
    }
    public async Task<UserProfileResponse> GetProfileAsync(string userId)
    {
        var profile = await _userManager.Users
            .Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();

        return profile;
    }
    public async Task<Result> UpdateUserProfileAsync(string userId,string username ,string firstName, string lastName)
    {
        var count = await _userManager.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(u => u.UserName, username)
                    .SetProperty(u => u.FirstName, firstName)
                    .SetProperty(u => u.LastName, lastName)
            );

        return count > 0
            ? Result.Success()
            : Result.Failure(UserErrors.UserNotFound);
    }
    public async Task<Result> ChabgeUserPasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<IEnumerable<UserResponse>> GetUsersInRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        return await _userManager.GetUsersInRoleAsync(role)
            .ContinueWith(t => t.Result.Select(u => u.Adapt<UserResponse>()), cancellationToken);
    }
}
