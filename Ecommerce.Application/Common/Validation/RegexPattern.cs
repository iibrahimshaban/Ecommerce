namespace Ecommerce.Application.Common.Validation;
public static class RegexPattern
{
    public const string Email = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
    public const string Password = "^(?=.*[0-9])(?=.*[!@#$%^&*()\\-_=+{};:,<.>/?])(?=.*[a-z])(?=.*[A-Z]).{8,}$";
    public const string UserName = "^[a-zA-Z][a-zA-Z0-9_]{2,15}$";
}
