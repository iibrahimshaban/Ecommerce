namespace Ecommerce.Application.Athuentication;
public interface IJwtProvider
{
    (string Token, int ExpiresIn) GenerateToken(ApplicationUserDto user, IEnumerable<string> Roles);
    string? ValidateToken(string token);
}
