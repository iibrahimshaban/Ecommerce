using Ecommerce.Application.Contracts.Cart;
using Ecommerce.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Member.Name)]
public class CartsController(ICartService cartService) : ControllerBase
{
    private readonly ICartService cartService = cartService;
    [HttpGet("")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await cartService.GetCartByUserIdAsync(userId! ,cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : result.ToProblem();
    }
    [HttpPost("items/")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemsRequest itemsRequest,
        IValidator<AddCartItemsRequest> validator , CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(itemsRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToProblem();
        }

        var userId = User.GetUserId();
        var result = await cartService.AddItemAsync(userId!, itemsRequest, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
    [HttpPut("items/{itemId}")]
    public async Task<IActionResult> UpdateItem(int itemId,
        [FromBody] UpdateCartItemRequest updateRequest,
        IValidator<UpdateCartItemRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(updateRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToProblem();
        }
        var userId = User.GetUserId();
        var result = await cartService.UpdateItemAsync(userId!, itemId, updateRequest, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int itemId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await cartService.RemoveItemAsync(userId!, itemId, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
    [HttpDelete("")]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await cartService.ClearCartAsync(userId!, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
