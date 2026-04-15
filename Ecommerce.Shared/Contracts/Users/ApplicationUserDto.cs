namespace Ecommerce.Shared.Contracts.Users;
public record ApplicationUserDto(
    string Id,
    string Email,
    string UserName,
    string FirstName,
    string LastName ,
    bool IsDisabled ,
    DateTimeOffset? LockoutEnd ,
    bool EmailConfirmed
    );
