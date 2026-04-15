namespace Ecommerce.Shared.Contracts.Auth;
public record SignInOutcome(
    bool Succeeded,
    bool IsLockedOut = false, 
    bool RequiresTwoFactor = false,
    bool IsNotAllowed = false
  );
