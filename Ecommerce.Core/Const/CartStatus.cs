namespace Ecommerce.Core.Const;
public enum  CartStatus
{
    Active = 0,      // Default: the cart is being used by the user
    CheckedOut = 1,  // Cart was converted into an Order
    Abandoned = 2,   // User left without checking out (optional feature)
    Expired = 3,     // Cart expired after a time limit (optional)
}
