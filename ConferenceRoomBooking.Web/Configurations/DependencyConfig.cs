using System.Text;
using ConferenceRoomBooking.Bll;
using ConferenceRoomBooking.Bll.Common.Shared.Abstractions;
using ConferenceRoomBooking.Bll.Common.Shared.Security;
using ConferenceRoomBooking.Bll.Common.Shared.Settings;
using ConferenceRoomBooking.Dal.SqlRepositories;
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
            .AddAutoMapperProfiles()
            .AddValidation()
            .AddHttpContextAccessor()
            .AddJwtAuthentication(config)
            .AddAppAuthorization()
            .AddAuthorization()
            .AddSwaggerDocs()
            .AddWebServices()
            .AddDalSqlRepositories()
            .AddBusinessLogic();
    }

    private static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        return services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();
    }

    private static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
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

    private static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();
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
