using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Analytics.Models;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Bll.Common.Bookings.Models;
using ConferenceRoomBooking.Bll.Common.Rooms.Models;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Web.Dtos.Analytics;
using ConferenceRoomBooking.Web.Dtos.Auth;
using ConferenceRoomBooking.Web.Dtos.Booking;
using ConferenceRoomBooking.Web.Dtos.Rooms;
using ConferenceRoomBooking.Web.Dtos.ServiceOptions;

namespace ConferenceRoomBooking.Web.Mapping;

public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<AuthResult, AuthResponse>()
            .ForCtorParam(nameof(AuthResponse.AppUser), opt => opt.MapFrom(src => new AppUserResponse(src.UserId.ToString(), src.Email)));

        CreateMap<ServiceOption, ServiceOptionResponse>();

        CreateMap<Room, RoomResponse>()
            .ForCtorParam(nameof(RoomResponse.BaseHourlyRate), opt => opt.MapFrom(src => src.BaseHourRate));

        CreateMap<AvailableRoom, AvailableRoomResponse>();

        CreateMap<BookedServiceOption, BookedServiceOptionResponse>();

        CreateMap<Booking, BookingResponse>()
            .ForCtorParam(nameof(BookingResponse.PriceBreakdown), opt => opt.MapFrom(src => new BookingPriceBreakdownResponse(
                src.BaseRoomCost,
                src.BaseRoomCost == 0 ? 0m : Math.Round((src.TotalPrice - src.ServicesCost) / src.BaseRoomCost * 100 - 100, 2),
                src.TotalPrice - src.ServicesCost,
                src.ServicesCost,
                src.TotalPrice)));

        CreateMap<RoomPerformance, RoomPerformanceResponse>();
        CreateMap<ServicePerformance, ServicePerformanceResponse>();
    }
}
