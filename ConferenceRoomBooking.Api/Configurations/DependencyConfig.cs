using System.Text;
using ConferenceRoomBooking.Api.Middleware;
using ConferenceRoomBooking.Api.Swagger;
using ConferenceRoomBooking.Application.BusinessLogic;
using ConferenceRoomBooking.Application.BusinessLogic.Auth;
using ConferenceRoomBooking.Application.BusinessLogic.Availability;
using ConferenceRoomBooking.Application.BusinessLogic.Pricing;
using ConferenceRoomBooking.Application.Orchestrators.Analytics;
using ConferenceRoomBooking.Application.Orchestrators.Auth;
using ConferenceRoomBooking.Application.Orchestrators.Booking;
using ConferenceRoomBooking.Application.Orchestrators.Rooms;
using ConferenceRoomBooking.Application.Orchestrators.ServiceOptions;
using ConferenceRoomBooking.Application.Security;
using ConferenceRoomBooking.Application.Settings;
using ConferenceRoomBooking.DataLayer;
using ConferenceRoomBooking.DataLayer.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Validators = ConferenceRoomBooking.Application.Validators.Validators;

namespace ConferenceRoomBooking.Api.Configurations;

public static class DependencyConfig
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();

        return services
            .AddExceptionHandler()
            .AddDb(config)
            .AddValidation()
            .AddIdentity()
            .AddJwtAuthentication(config)
            .AddAppAuthorization()
            .AddAuthorization()
            .AddSwaggerDocs()
            .AddBusinessLogic()
            .AddOrchestrators();
    }

    private static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        return services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();
    }

    private static IServiceCollection AddDb(this IServiceCollection services, IConfiguration config)
    {
        return services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
    }
    
    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(Validators).Assembly);
        services.AddFluentValidationAutoValidation();
        return services;
    }

    private static IServiceCollection AddOrchestrators(this IServiceCollection services)
    {
        services.AddScoped<IRoomOrchestrator, RoomOrchestrator>();
        services.AddScoped<IServiceOptionOrchestrator, ServiceOptionOrchestrator>();
        services.AddScoped<IBookingOrchestrator, BookingOrchestrator>();
        services.AddScoped<IAuthOrchestrator, AuthOrchestrator>();
        services.AddScoped<IAnalyticsOrchestrator, AnalyticsOrchestrator>();
        return services;
    }
    
    private static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddSingleton<IRentalPriceCalculator, RentalPriceCalculator>();
        services.AddSingleton<IRoomAvailabilityChecker, RoomAvailabilityChecker>();
        services.AddTransient<IJwtIssuer, JwtIssuer>();
        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>(options => 
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6; 
            options.Password.RequireNonAlphanumeric = false; 
        }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        
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