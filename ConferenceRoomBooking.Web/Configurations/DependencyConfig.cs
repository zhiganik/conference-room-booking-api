using System.Text;
using ConferenceRoomBooking.Bll.Analytics;
using ConferenceRoomBooking.Bll.Auth;
using ConferenceRoomBooking.Bll.Bookings;
using ConferenceRoomBooking.Bll.Common.Analytics;
using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.Shared.Abstractions;
using ConferenceRoomBooking.Bll.Common.Shared.Security;
using ConferenceRoomBooking.Bll.Common.Shared.Settings;
using ConferenceRoomBooking.Bll.Rooms;
using ConferenceRoomBooking.Bll.ServiceOptions;
using ConferenceRoomBooking.Dal.SqlRepositories.Analytics;
using ConferenceRoomBooking.Dal.SqlRepositories.Auth;
using ConferenceRoomBooking.Dal.SqlRepositories.Bookings;
using ConferenceRoomBooking.Dal.SqlRepositories.Rooms;
using ConferenceRoomBooking.Dal.SqlRepositories.ServiceOptions;
using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using ConferenceRoomBooking.Web.Middleware;
using ConferenceRoomBooking.Web.Services;
using ConferenceRoomBooking.Web.Swagger;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using ConferenceRoomBooking.Web.Validators;

namespace ConferenceRoomBooking.Web.Configurations;

public static class DependencyConfig
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();

        return services
            .AddExceptionHandler()
            .AddSqlConnectionFactory()
            .AddAutoMapperProfiles()
            .AddValidation()
            .AddHttpContextAccessor()
            .AddJwtAuthentication(config)
            .AddAppAuthorization()
            .AddAuthorization()
            .AddSwaggerDocs()
            .AddBusinessLogic()
            .AddServices()
            .AddRepositories()
            .AddManagers();
    }

    private static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        return services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();
    }

    private static IServiceCollection AddSqlConnectionFactory(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
        return services;
    }

    private static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        // Scans the Dal.SqlRepositories (Entity <-> Model) and Web (Model <-> Dto) assemblies for Profiles.
        services.AddAutoMapper(cfg => { },
            typeof(ConferenceRoomBooking.Dal.SqlRepositories.Mapping.AutomapperConfig).Assembly,
            typeof(ConferenceRoomBooking.Web.Mapping.AutomapperConfig).Assembly);
        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ValidatorsAssemblyMarker).Assembly);
        services.AddFluentValidationAutoValidation();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IRoomBookingLock, RoomBookingLock>();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IServiceOptionRepository, ServiceOptionRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        return services;
    }

    private static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<IRoomManager, RoomManager>();
        services.AddScoped<IServiceOptionManager, ServiceOptionManager>();
        services.AddScoped<IBookingManager, BookingManager>();
        services.AddScoped<IAuthManager, AuthManager>();
        services.AddScoped<IAnalyticsManager, AnalyticsManager>();
        return services;
    }

    private static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddSingleton<IRentalPriceCalculator, RentalPriceCalculator>();
        services.AddTransient<IJwtIssuer, JwtIssuer>();
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));

        var jwtSettings = config.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                          ?? throw new InvalidOperationException("Jwt configuration section is missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey
                    ?? throw new ArgumentException("Jwt:Key cannot be null or empty")))
            };
        });

        return services;
    }

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.RequireAdmin, policy => policy.RequireRole(Roles.Admin))
            .AddPolicy(AuthorizationPolicies.RequireUser, policy => policy.RequireRole(Roles.User, Roles.Admin));

        return services;
    }

    private static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Conference Room Booking API",
                Version = "v1",
                Description = "API for managing conference rooms, bookings, and rental pricing."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT access token. Do NOT include the word 'Bearer' — Swagger adds it automatically."
            });

            options.OperationFilter<AuthorizeOperationFilter>();
        });

        return services;
    }
}
