using Ecommerce.Shared.Contracts.Users;

namespace Ecommerce.Shared.Contracts.Auth;
public record ConfirmationCodeDto(
    ApplicationUserDto User,
    string Code,
    string UserId
    );

