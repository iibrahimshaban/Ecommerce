using Ecommerce.Application.Contracts.Users;
using Ecommerce.Shared.Authorization;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Ecommerce.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Admin.Name)]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("email/{email}", Name = "GetUserByEmail")]
    public async Task<IActionResult> GetByEmail([FromRoute] string email, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByEmailAsync(email, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request,
        [FromServices] IValidator<CreateUserRequest> validator,CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();


        var result = await _userService.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtRoute("GetUserByEmail",new { email = result.Value.Email }, result.Value)
            : result.ToProblem();
    }
    [HttpPut("{UserId}")]
    public async Task<IActionResult> Update([FromRoute] string UserId, 
        [FromBody] UpdateUserRequest request,
        [FromServices] IValidator<UpdateUserRequest> validator,CancellationToken cancellationToken)
    {

        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();


        var result = await _userService.UpdateUserAsync(UserId, request, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
    [HttpPut("{UserId}/toggle-status")]
    public async Task<IActionResult> ToggleUser([FromRoute] string UserId)
    {
        var result = await _userService.ToggleStatus(UserId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
    [HttpPut("{UserId}/unlock")]
    public async Task<IActionResult> UnLockUser([FromRoute] string UserId)
    {
        var result = await _userService.UnlockAsync(UserId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
