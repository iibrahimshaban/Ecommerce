namespace Ecommerce.Application.Contracts.Users;
public record ChangePasswordRequest(
    string Currentpassword,
    string Newpassword
    );
