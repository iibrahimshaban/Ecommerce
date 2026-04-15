
using Ecommerce.Application.Contracts.Orders;
using Ecommerce.Core.Const;

namespace Ecommerce.Application.Services;
public class OrderService(IUnitOfWork unitOfWork) : IOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders.FindAllWithIncludesAsync(o => o.UserId == userId,
                cancellationToken,
                "OrderItems", 
                "OrderItems.Product"
                );


        return orders.Adapt<IEnumerable<OrderResponse>>();
    }
    public async Task<Result<OrderResponse>> GetOrderByIdAsync(int orderId, string userId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.FindWithIncludesAsync(o => o.Id == orderId && o.UserId == userId,
                cancellationToken,
                 "OrderItems",
                "OrderItems.Product"
                );

        if (order is null)
            return Result.Failure<OrderResponse>(OrderErrors.OrderNotFound);

        return Result.Success(order.Adapt<OrderResponse>());
    }

    public async Task<Result<OrderResponse>> CheckOutAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _unitOfWork.Carts.FindWithIncludesAsync(c => c.UserId == userId, 
            cancellationToken ,
             "CartItems",
             "CartItems.Product");

        if (cart is null || cart.CartItems.Count == 0)
            return Result.Failure<OrderResponse>(OrderErrors.EmptyCart);

        var products = await _unitOfWork.Products.FindAll(p => cart.CartItems
                            .Select(ci => ci.ProductId).Contains(p.Id), cancellationToken );

       var avaliableStocks = products.Select(x => new { x.Id, x.Stock });

        foreach (var item in cart.CartItems)
        {
            var productStock = avaliableStocks.FirstOrDefault(x => x.Id == item.ProductId)?.Stock ?? 0;
            if (item.Quantity > productStock)
                return Result.Failure<OrderResponse>(OrderErrors.InsufficientStock);
        }
        foreach (var item in cart.CartItems)
        {
            var product = products.First(p => p.Id == item.ProductId);
            product.Stock -= item.Quantity;
        }

        var order = cart.Adapt<Order>();
        await _unitOfWork.Orders.AddAsync(order, cancellationToken);

        _unitOfWork.CartItems.RemoveRange(cart.CartItems);
        cart.TotalPrice = 0;
        cart.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(order.Adapt<OrderResponse>());
    }
    public async Task<Result> CancelOrderAsync(string userId, int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.Find(x => x.UserId == userId && x.Id == orderId,cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.OrderNotFound);

        if (order.Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.OrderAlreadyProcessed);

        order.Status = OrderStatus.Cancelled;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
    public async Task<Result> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.Find(o => o.Id == orderId, cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.OrderNotFound);

        order.Status = (OrderStatus)request.Status;

        _unitOfWork.Orders.Update(order);
        var rows = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rows > 0
            ? Result.Success()
            : Result.Failure(OrderErrors.OrderNotFound);
    }
}
