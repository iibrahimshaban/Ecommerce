using Ecommerce.Application.Contracts.Orders;
using Ecommerce.Core.Const;

namespace Ecommerce.Application.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> GetOrderByIdAsync(int orderId, string userId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> CheckOutAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result> CancelOrderAsync(string userId,int orderId, CancellationToken cancellationToken = default);
    Task<Result> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
}