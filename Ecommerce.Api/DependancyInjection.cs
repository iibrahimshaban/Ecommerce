using Ecommerce.Application;
using Ecommerce.Application.Athuentication;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.Common.Options;
using Ecommerce.Core.Interfaces;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Repositories;
using Ecommerce.Infrastructure.Services;
using Ecommerce.Infrastructure.UnitOfWork;
using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Api;

public static class DependancyInjection
{
    public static IServiceCollection AddApiDependancies(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddControllers();

        services
            .AddDbContextConfiguration(configuration)
            .AddRepositoryServices()
            .AddOpenApi()
            .AddJwtBearerTokenConfiguration(configuration)
            .AddOptionPatternConfiguration()
            .RegisterMappings()
            .AddBackGroundJobs(configuration);

        services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

        return services; ;
    }
    private static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
             sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, ApplicationRole>()
         .AddEntityFrameworkStores<ApplicationDbContext>()
         .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }
    private static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddHttpContextAccessor();

        return services;
    }
    public static IServiceCollection AddOptionPatternConfiguration(this IServiceCollection services)
    {
        services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services
               .AddOptions<EmailOptions>()
               .BindConfiguration(EmailOptions.SectionName)
               .ValidateDataAnnotations()
               .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddJwtBearerTokenConfiguration(this IServiceCollection services,IConfiguration configuration)
    {
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        var settings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
         .AddJwtBearer(o =>
         {
             o.SaveToken = true;
             o.TokenValidationParameters = new TokenValidationParameters
             {
                 ValidateIssuerSigningKey = true,
                 ValidateIssuer = true,
                 ValidateAudience = true,
                 ValidateLifetime = true,
                 IssuerSigningKey = new 
                 SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings!.Key)),
                 RoleClaimType = ClaimTypes.Role,
                 ValidIssuer = settings.Issuer,
                 ValidAudience = settings.Audience
             };
         });


        return services;
    }
    public static IServiceCollection RegisterMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(UserMappingConfig).Assembly); 
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
    public static IServiceCollection AddBackGroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
         .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
         .UseSimpleAssemblyNameTypeSerializer()
         .UseRecommendedSerializerSettings()
         .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
        return services;
    }
}
