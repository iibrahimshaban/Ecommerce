using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Contracts.Users;

namespace Ecommerce.Application.Services;
public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await _userRepository.GetAllUsersAsync(cancellationToken);
    }
    public async Task<Result<UserResponse>> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var userRoles = await _userRepository.GetUserRolesById(userId, cancellationToken);

        var response = user.Adapt<UserResponse>() with
        {
            Roles = userRoles
        };
        return Result.Success(response);
    }
    public async Task<Result<UserResponse>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByEmailAsync(email, cancellationToken);

        if (user == null)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var userRoles = await _userRepository.GetUserRolesById(user.Id, cancellationToken);

        var response = user.Adapt<UserResponse>() with
        {
            Roles = userRoles
        };

        return Result.Success(response);
    }
    public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var UserIsExists = await _userRepository.UserEmailExists(request.Email,cancellationToken:cancellationToken);

        if (UserIsExists)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var UserNameIsExists = await _userRepository.UserEmailExists(request.UserName,cancellationToken: cancellationToken);

        if (UserNameIsExists)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedUsername);


        var user = request.Adapt<ApplicationUserDto>() with { EmailConfirmed = true };

        var existingRoles = await _userRepository.GetRoles(cancellationToken);

        if (request.Roles.Except(existingRoles).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var result = await _userRepository.CreateUserAsync(user, request.Password,cancellationToken);

        if (result.IsSuccess)
        {

            await _userRepository.AddRolesToUserAsync(request.Email, request.Roles, cancellationToken);

            var response = result.Value.Adapt<UserResponse>() with
            {
                Roles = request.Roles
            };

            return Result.Success(response);

        }
        
        return Result.Failure<UserResponse>(result.Error);

    }
    public async Task<Result> UpdateUserAsync(string UserId, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(UserId, cancellationToken);

        if ( user == null)
            return Result.Failure(UserErrors.UserNotFound);

        var UserIsExists = await _userRepository.UserEmailExists(request.Email,UserId,cancellationToken);

        if (UserIsExists)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var UserNameIsExists = await _userRepository.UserEmailExists(request.UserName,UserId, cancellationToken);

        if (UserNameIsExists)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedUsername);

        var existingRoles = await _userRepository.GetRoles(cancellationToken);

        if (request.Roles.Except(existingRoles).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        user = request.Adapt(user);

        var result = await _userRepository.UpdateUserAsync(user);

        if (result.IsSuccess)
        {
            await _userRepository.UpdateRolesToUserAsync(user.Email, request.Roles, cancellationToken);

            return Result.Success();
        }

        return Result.Failure(result.Error);

    }
    public async Task<Result> ToggleStatus(string userId)
    {
       return await _userRepository.ToggleUserStatusAsync(userId);
    }
    public async Task<Result> UnlockAsync(string userId)
    {
       return await _userRepository.UnlockUserAsync(userId);
    }
    public async Task<UserProfileResponse> GetProfileAsync(string UserId)
    {
        return await _userRepository.GetProfileAsync(UserId);
    }
    public async Task<Result> UpdateProfileAsync(string UserId, UpdateProfileRequest request)
    {
        var userNameExists = await _userRepository.UserNameExists(request.UserName);

        if (userNameExists)
            return Result.Failure(UserErrors.DuplicatedUsername);

        var result = await _userRepository.UpdateUserProfileAsync(UserId,request.UserName ,request.FirstName, request.LastName);

        return result;
    }
    public async Task<Result> ChangePasswordAsync(string UserId, ChangePasswordRequest request)
    {
        return await _userRepository.ChabgeUserPasswordAsync(UserId, request.Currentpassword, request.Newpassword);

    }
}
