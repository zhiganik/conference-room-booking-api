using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Web.Dtos.Auth;
using ConferenceRoomBooking.Web.Dtos.ServiceOptions;

namespace ConferenceRoomBooking.Web.Mapping;

public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<AuthResult, AuthResponse>()
            .ForMember(dest => dest.AppUser, opt => opt.MapFrom(src => new AppUserResponse(src.UserId.ToString(), src.Email)));

        CreateMap<ServiceOption, ServiceOptionResponse>();
    }
}
