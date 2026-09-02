using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Bll.Common.Bookings.Models;
using ConferenceRoomBooking.Bll.Common.Rooms.Models;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Auth.Entities;
using ConferenceRoomBooking.Dal.SqlRepositories.Bookings.Entities;
using ConferenceRoomBooking.Dal.SqlRepositories.Rooms.Entities;
using ConferenceRoomBooking.Dal.SqlRepositories.ServiceOptions.Entities;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Mapping;

// Entity -> Model maps are added feature by feature as each repository is implemented.
public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<UserEntity, User>();
        CreateMap<ServiceOptionEntity, ServiceOption>();

        CreateMap<RoomEntity, Room>()
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.ServiceOptions));

        CreateMap<BookingServiceOptionEntity, BookedServiceOption>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ServiceOptionName));

        CreateMap<BookingEntity, Booking>()
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.ServiceOptions));
    }
}
