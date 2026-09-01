using AutoMapper;

namespace ConferenceRoomBooking.Web.Mapping;

// Model -> Dto (and Dto -> Model) maps are added feature by feature as each controller is wired up.
public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
    }
}
