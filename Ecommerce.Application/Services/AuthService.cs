using Ecommerce.Application.Athuentication;
using Ecommerce.Application.Common.Helpers;
using Ecommerce.Application.Common.Interfaces;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;
using Ecommerce.Shared.Contracts.Auth;


namespace Ecommerce.Application.Services;
public class AuthService(IUserRepository user ,IJwtProvider jwtProvider ,
    IHttpContextAccessor httpContextAccessor , IEmailSender emailSender , ILogger<AuthService> logger ) : IAuthService
{
    private readonly int _RefreshTokenExpiryDays = 14;

    private readonly IUserRepository _userRepository = user;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByEmailAsync(Email, cancellationToken);

       
        if (user == null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredintials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var result = await _userRepository.PasswordSignInAsync(user.UserName, Password);

        if (result.Succeeded)
        {
            var userRoles = await _userRepository.GetUserRolesById(user.Id, cancellationToken);

            var (Token, ExpiresIn) = _jwtProvider.GenerateToken(user,userRoles);

            var (RefreshToken, RefreshTokenExpiryDate) = await _userRepository
                .GenerateRfreshTokenAsync(_RefreshTokenExpiryDays, user.Id,cancellationToken);

            var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, Token, ExpiresIn,
                RefreshToken, RefreshTokenExpiryDate);

            return Result.Success(response);
        }

        if (result.IsLockedOut)
            return Result.Failure<AuthResponse>(UserErrors.LockedOutUser);

        return Result.Failure<AuthResponse>(result.IsNotAllowed
                    ? UserErrors.EmailNotConfirmed
                    : UserErrors.InvalidCredintials);
    }

    public async Task<Result> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken)
    {
        var UserIsExists = await _userRepository.UserEmailExists(request.Email,cancellationToken: cancellationToken);

        if (UserIsExists)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var UserNameIsExists = await _userRepository.UserNameExists(request.UserName,cancellationToken: cancellationToken);

        if (UserNameIsExists)
            return Result.Failure(UserErrors.DuplicatedUsername);

        var result = await _userRepository.CreateUserAsync(request.Adapt<ApplicationUserDto>(), request.Password,cancellationToken);

        if (result.IsSuccess)
        {
            var codeResult = await _userRepository.GenerateConfirmationCodeAsync(request.Email, cancellationToken);

            await SendConfirmationEmailAsync(codeResult,cancellationToken);

            return Result.Success();
        }

        return Result.Failure(result.Error);

    }
    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var UserId = _jwtProvider.ValidateToken(request.Token);

        if (UserId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshCredintials);

        var User = await _userRepository.GetByIdAsync(UserId,cancellationToken);

        if (User is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshCredintials);

        if (User.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (User.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedOutUser);

        var UserRefrechTokenResult = await _userRepository.CurrentRefreshTokenAnyAsync(UserId, request.RefreshToken);

        if (!UserRefrechTokenResult)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshCredintials);

        var userRoles= await _userRepository.GetUserRolesById(UserId, cancellationToken);

        var (NewToken, ExpiresIn) = _jwtProvider.GenerateToken(User, userRoles);

        var (RefreshToken, RefreshTokenExpiryDate) = await _userRepository
                .GenerateRfreshTokenAsync(_RefreshTokenExpiryDays, UserId, cancellationToken);

        return Result.Success(new AuthResponse (User.Id, User.Email, User.FirstName, User.LastName, NewToken, ExpiresIn,
            RefreshToken, RefreshTokenExpiryDate));

    }
    public async Task<Result> RevokeRefreshTokenAsync(RefreshTokenRequest request,
            CancellationToken cancellationToken = default)
    {
        var UserId = _jwtProvider.ValidateToken(request.Token);

        if (UserId is null)
            return Result.Failure(UserErrors.InvalidRefreshCredintials);

        var User = await _userRepository.GetByIdAsync(UserId, cancellationToken);

        if (User is null)
            return Result.Failure(UserErrors.InvalidRefreshCredintials);

        var UserRefreshTokenResult = await _userRepository.CurrentRefreshTokenAnyAsync(UserId, request.RefreshToken);

        if (!UserRefreshTokenResult)
            return Result.Failure(UserErrors.InvalidRefreshCredintials);

        return Result.Success();
    }

    
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        var User = await _userRepository.GetByIdAsync(request.UserId,cancellationToken);

        if (User == null)
            return Result.Failure(UserErrors.InvalidCode);

        if (User.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = request.Code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userRepository.ConfirmEmailAsync(User.Id, code,cancellationToken);

        if (result.IsSuccess)
            return Result.Success();

        return Result.Failure(result.Error);

    }
    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (user == null)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var codeDto = await _userRepository.GenerateConfirmationCodeAsync(request.Email,cancellationToken);

        await SendConfirmationEmailAsync(codeDto,cancellationToken);

        return Result.Success();
    }
    public async Task<Result> SendResetPasswordCodeAsync(ForgetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var codeDto = await _userRepository.GeneratePasswordResetTokenAsync(request.Email, cancellationToken);

        await SendResetPasswordEmailAsync(codeDto,cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed with { StatusCode = StatusCodes.Status400BadRequest });

        var result = await _userRepository.ResetPasswordAsync(request.Email,request.NewPassword,request.Code,cancellationToken);

        return result.IsSuccess
            ? Result.Success()
            : Result.Failure(result.Error);

    }

    private async Task SendConfirmationEmailAsync(ConfirmationCodeDto codeDto, CancellationToken cancellationToken = default)
    {
        //we won't use origin as i can't send url with the headers 
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Host;

        _logger.LogInformation("url is {origin}", origin);

        var EmailBody = EmailBodyBuilder.GenerateEmailBody("WelcomeEmail",
            new Dictionary<string, string>
            {
                        { "{{name}}",codeDto.User.FirstName+" "+codeDto.User.LastName},
                        { "{{action_url}}",$"https://{origin}/Auth/Confirm-Email?userId={codeDto.UserId}&code={codeDto.Code}" }
            });

        BackgroundJob.Enqueue(() =>
        _emailSender.SendEmailAsync(codeDto.User.Email, "✅ Survay basket : email verification ", EmailBody, cancellationToken));

        await Task.CompletedTask;

    }

    private async Task SendResetPasswordEmailAsync(ResetPassowrdDto dto, CancellationToken cancellationToken)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Host;

        _logger.LogInformation("url is {origin}", origin);
        var EmailBody = EmailBodyBuilder.GenerateEmailBody("ForgotPassword",
            new Dictionary<string, string>
            {
                        { "{{name}}",dto.User.FirstName+" "+dto.User.LastName},
                        { "{{action_url}}",$"https://{origin}/Auth/forget-password?email={dto.User.Email}&code={dto.Code}" }
            });

        BackgroundJob.Enqueue(() =>
        _emailSender.SendEmailAsync(dto.User.Email!, "✅ Survay basket : reset password ", EmailBody, cancellationToken));

        await Task.CompletedTask;
    }
}
