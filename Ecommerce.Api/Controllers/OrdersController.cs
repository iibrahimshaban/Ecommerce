using Ecommerce.Application.Contracts.Orders;
using Ecommerce.Core.Const;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;
    [HttpGet("")]
    [Authorize(Roles = DefaultRoles.Member.Name)]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var orders = await _orderService.GetOrdersByUserIdAsync(userId!, cancellationToken);
        return Ok(orders);
    }
    [HttpGet("{orderId}")]
    [Authorize(Roles = DefaultRoles.Member.Name)]
    public async Task<IActionResult> GetOrder(int orderId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _orderService.GetOrderByIdAsync(orderId, userId!, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
    [HttpPost("checkout")]
    [Authorize(Roles = DefaultRoles.Member.Name)]
    public async Task<IActionResult> CheckOut(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _orderService.CheckOutAsync(userId!, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
    [HttpPut("cancel/{orderId}")]
    [Authorize(Roles = DefaultRoles.Member.Name)]
    public async Task<IActionResult> Cancel(int orderId, CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        var result = await _orderService.CancelOrderAsync(userId!, orderId, cancellationToken);

        return result.IsSuccess
            ? NoContent() 
            : result.ToProblem();
    }
    [HttpGet("statuses")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public IActionResult GetOrderStatuses()
    {
        var statuses = Enum.GetValues<OrderStatus>()
            .Cast<OrderStatus>()
            .Select(s => new
            {
                value = (int)s,
                name = s.ToString()
            });

        return Ok(statuses);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> UpdateOrderStatus(int id,UpdateOrderStatusRequest request,CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id,request, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

}
