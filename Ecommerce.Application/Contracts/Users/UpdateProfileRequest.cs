namespace Ecommerce.Application.Contracts.Users;
public record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string UserName
    );

