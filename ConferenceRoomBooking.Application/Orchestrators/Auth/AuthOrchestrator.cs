using ConferenceRoomBooking.Application.BusinessLogic.Auth;
using ConferenceRoomBooking.Application.Dtos.Auth;
using ConferenceRoomBooking.Application.Exceptions;
using ConferenceRoomBooking.Application.Security;
using ConferenceRoomBooking.Application.Settings;
using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ConferenceRoomBooking.Application.Orchestrators.Auth;

public class AuthOrchestrator(UserManager<AppUser> userManager, 
    IJwtIssuer jwtIssuer, IOptions<JwtSettings> jwtOptions) : IAuthOrchestrator
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        
        if (existingUser is not null)
            throw new ConflictException($"A user with email '{request.Email}' already exists.");
        
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
        };

        var result = await userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ConflictException($"Registration failed: {errors}");
        }
        await userManager.AddToRoleAsync(user, Roles.User);

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        if (user is null)
            throw new UnauthorizedException("Invalid email or password.");

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        
        if (!passwordValid)
            throw new UnauthorizedException("Invalid email or password.");
        
        return await BuildAuthResponseAsync(user);
    }
    
    private async Task<AuthResponse> BuildAuthResponseAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtIssuer.GenerateAccessToken(user, roles);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenExpirationMinutes);

        return new AuthResponse(accessToken, expiresAtUtc, new AppUserResponse(user.Id, user.Email));
    }
}