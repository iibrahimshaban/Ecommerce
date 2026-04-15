namespace Ecommerce.Application.Contracts.Auth;
public record RegistrationRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string UserName
    );
