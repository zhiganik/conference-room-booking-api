using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Web.Dtos.Auth;

namespace ConferenceRoomBooking.Web.Mapping;

// Model -> Dto (and Dto -> Model) maps are added feature by feature as each controller is wired up.
public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<AuthResult, AuthResponse>()
            .ForMember(dest => dest.AppUser, opt => opt.MapFrom(src => new AppUserResponse(src.UserId.ToString(), src.Email)));
    }
}
