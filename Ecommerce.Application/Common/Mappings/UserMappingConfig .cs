using Ecommerce.Application.Contracts.Users;
using Mapster;

namespace Ecommerce.Application.Common.Mappings;
public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegistrationRequest, ApplicationUserDto>()
            .Map(dest => dest.IsDisabled, _ => false)
            .Ignore(dest => dest.Id);

        config.NewConfig<(ApplicationUserDto user, IList<string> Roles), UserResponse>()
                .Map(dest => dest, src => src.user)
                .Map(dest => dest.Roles, src => src.Roles);
    }
}
