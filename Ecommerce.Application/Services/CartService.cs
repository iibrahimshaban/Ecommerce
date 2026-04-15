using Ecommerce.Application.Contracts.Cart;
using Ecommerce.Application.Contracts.Orders;
using System.Diagnostics.Contracts;

namespace Ecommerce.Application.Services;
public class CartService(IUnitOfWork unitOfWork) : ICartService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CartResponse>> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _unitOfWork.Carts.ProjectToType<CartResponse>(x => x.UserId == userId, cancellationToken);

        if (cart is null)
            return Result.Failure<CartResponse>(CartErrors.NotFound);

        return Result.Success(cart);

    }
    public async Task<Result<CartResponse>> AddItemAsync(string userId,AddCartItemsRequest itemsRequest ,CancellationToken cancellationToken = default)
    {
        var existingCart = await _unitOfWork.Carts.Find(c => c.UserId == userId,cancellationToken , x => x.CartItems);

        if (existingCart is null)
        {
            var cart = new Cart
            {
                UserId = userId,
                CartItems = itemsRequest.Items
                    .Select(i => new CartItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                    }).ToList(),
                TotalPrice = await CalculateTotalPriceAsync(itemsRequest.Items, cancellationToken)
            };
            await _unitOfWork.Carts.AddAsync(cart, cancellationToken);
        }
        else
        {
            foreach (var i in itemsRequest.Items)
            {
                var existingItem = existingCart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == i.ProductId);

                if (existingItem is not null)
                {
                    existingItem.Quantity += i.Quantity;
                }
                else
                {
                    existingCart.CartItems.Add(new CartItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    });
                }
            }

            existingCart.TotalPrice = await CalculateTotalPriceAsync(
                existingCart.CartItems.Select(ci => new AddItemRequest(ci.ProductId, ci.Quantity))
                , cancellationToken);

            existingCart.UpdatedAt = DateTime.UtcNow;
        }

        var rows = await _unitOfWork.SaveChangesAsync(cancellationToken);
        var response = await _unitOfWork.Carts.ProjectToType<CartResponse>(x => x.UserId == userId,cancellationToken);

        return rows > 0 
            ? Result.Success(response) 
            : Result.Failure<CartResponse>(CartErrors.CreateFailed);
    }
    public async Task<Result> UpdateItemAsync(string userId, int cartItemId, UpdateCartItemRequest updateRequest, CancellationToken cancellationToken = default)
    {
        var cartItem = await _unitOfWork.CartItems
            .Find(ci => ci.Id == cartItemId && ci.Cart.UserId == userId ,
            cancellationToken,
            ci => ci.Cart,     
            ci => ci.Product,  
            ci => ci.Cart.CartItems
            );

        if (cartItem is null)
            return Result.Failure(CartErrors.NotFound);

        cartItem.Quantity = updateRequest.Quantity;

        cartItem.Cart.TotalPrice = await CalculateTotalPriceAsync(
            cartItem.Cart.CartItems.Select(ci => new AddItemRequest(ci.ProductId, ci.Quantity))
            , cancellationToken);

        cartItem.Cart.UpdatedAt = DateTime.UtcNow;
        var rows = await _unitOfWork.SaveChangesAsync(cancellationToken);
        return rows > 0 
            ? Result.Success() 
            : Result.Failure(CartErrors.UpdateFailed);
    }
    public async Task<Result> RemoveItemAsync(string userId, int cartItemId, CancellationToken cancellationToken = default)
    {
        var cart = await _unitOfWork.Carts.FindWithIncludesAsync(
                c => c.UserId == userId,
                cancellationToken,
                "CartItems",
                "CartItems.Product");

        if (cart is null)
            return Result.Failure(CartErrors.NotFound);

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

        if (cartItem is null)
            return Result.Failure(CartErrors.NotFound);

        _unitOfWork.CartItems.Remove(cartItem);

        cart.TotalPrice = cart.CartItems
               .Where(ci => ci.Id != cartItemId)
               .Sum(ci => ci.Quantity * ci.Product.Price);

        cart.UpdatedAt = DateTime.UtcNow;

        var rows = await _unitOfWork.SaveChangesAsync(cancellationToken);
        return rows > 0 
            ? Result.Success() 
            : Result.Failure(CartErrors.DeleteFailed);
    }

    public async Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _unitOfWork.Carts
        .Find(c => c.UserId == userId,
              cancellationToken,
              c => c.CartItems);

        if (cart is null)
            return Result.Failure(CartErrors.NotFound);

        _unitOfWork.CartItems.RemoveRange(cart.CartItems);
        cart.TotalPrice = 0;
        cart.UpdatedAt = DateTime.UtcNow;

        var rows = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rows > 0
            ? Result.Success()
            : Result.Failure(CartErrors.DeleteFailed);
    }
    
    private async Task<double> CalculateTotalPriceAsync(IEnumerable<AddItemRequest> cartItems, CancellationToken cancellationToken = default)
    {
        var totalPrice = 0.0;
        var products = await _unitOfWork.Products
                .FindAll(x => cartItems
                .Select(i => i.ProductId).Contains(x.Id), cancellationToken);

        foreach (var item in products)
        {
            totalPrice += item.Price * cartItems.First(x => x.ProductId == item.Id).Quantity;
        }

        return totalPrice;
    }
}
