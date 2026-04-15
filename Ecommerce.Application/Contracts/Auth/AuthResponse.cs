namespace Ecommerce.Application.Contracts.Auth;
public record AuthResponse(
    string Id,
    string? Email,
    string FName,
    string LName,
    string Token,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpirationdate
 );
