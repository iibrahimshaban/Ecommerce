using Ecommerce.Application.Contracts.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request,[FromServices] IValidator<LoginRequest> validator, 
        CancellationToken cancellationToken)
    {
       var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();
        

        var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return  result.IsSuccess 
            ? Ok(result.Value) 
            : result.ToProblem();
    }
    [HttpPost("signup")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request,
        [FromServices] IValidator<RegistrationRequest> validator, CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var result = await _authService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess 
            ? NoContent()
            : result.ToProblem();
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refresh,
        [FromServices] IValidator<RefreshTokenRequest> validator,
            CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(refresh, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var AuthResult = await _authService.GetRefreshTokenAsync(refresh, cancellationToken);

        return AuthResult.IsSuccess
            ? Ok(AuthResult.Value) 
            : AuthResult.ToProblem();
    }
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest refresh,
        [FromServices] IValidator<RefreshTokenRequest> validator,
        CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(refresh, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var result = await _authService.RevokeRefreshTokenAsync(refresh, cancellationToken);

        return result.IsSuccess
            ? Ok(result.IsSuccess) 
            : result.ToProblem();
    }
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request 
        , [FromServices] IValidator<ConfirmEmailRequest> validator
        , CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var result = await _authService.ConfirmEmailAsync(request, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request
        , [FromServices] IValidator<ResendConfirmationEmailRequest> validator
        , CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var result = await _authService.ResendConfirmationEmailAsync(request, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
    [HttpPost("forget-password")]
    public async Task<IActionResult> Forgetpassword([FromBody] ForgetPasswordRequest request
        , [FromServices]  IValidator<ForgetPasswordRequest> validator
           , CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var result = await _authService.SendResetPasswordCodeAsync(request, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request
        ,[FromServices] IValidator<ResetPasswordRequest> validator
        , CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var result = await _authService.ResetPasswordAsync(request, cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
} 
