using AutoMapper;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Dal.SqlRepositories.Auth.Entities;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Mapping;

// Entity -> Model maps are added feature by feature as each repository is implemented.
public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<UserEntity, User>();
    }
}
