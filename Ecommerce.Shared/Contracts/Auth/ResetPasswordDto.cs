using Ecommerce.Shared.Contracts.Users;

namespace Ecommerce.Shared.Contracts.Auth;
public record ResetPassowrdDto(
    ApplicationUserDto User,
    string Code,
    string UserId
    );
