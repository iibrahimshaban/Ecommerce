using Ecommerce.Application.Contracts.Users;
using Ecommerce.Shared.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountsController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> Info()
    {
        var result = await _userService.GetProfileAsync(User.GetUserId()!);

        return Ok(result);
    }
    [HttpPut("info")]
    public async Task<IActionResult> Update([FromBody] UpdateProfileRequest request ,
        [FromServices] IValidator<UpdateProfileRequest> validator)
    {
        var ValidatorResult = await validator.ValidateAsync(request);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();


        var result = await _userService.UpdateProfileAsync(User.GetUserId()!, request);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request ,
        [FromServices] IValidator<ChangePasswordRequest> validator , CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();


        var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
