using System.IdentityModel.Tokens.Jwt;
using ConferenceRoomBooking.Application.Services;

namespace ConferenceRoomBooking.Api.Services
{
    public class UserContext(IHttpContextAccessor contextAccessor) : IUserContext
    {
        public string? UserId => contextAccessor.HttpContext?.User
            .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        public bool IsAuthenticated => contextAccessor.HttpContext?.User
            .Identity?.IsAuthenticated ?? false;
    }
}
