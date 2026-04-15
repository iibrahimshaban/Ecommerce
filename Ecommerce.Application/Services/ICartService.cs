using Ecommerce.Application.Contracts.Cart;

namespace Ecommerce.Application.Services;
public interface ICartService
{
    Task<Result<CartResponse>> GetCartByUserIdAsync(string userId,CancellationToken cancellationToken = default);
    Task<Result<CartResponse>> AddItemAsync(string userId, AddCartItemsRequest itemsRequest, CancellationToken cancellationToken = default);
    Task<Result> UpdateItemAsync(string userId, int cartItemId, UpdateCartItemRequest updateRequest, CancellationToken cancellationToken = default);
    Task<Result> RemoveItemAsync(string userId, int cartItemId, CancellationToken cancellationToken = default);
    Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default);

}
