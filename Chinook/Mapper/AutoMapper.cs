using AutoMapper;

namespace Chinook.Mapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Chinook.Models.Album, Chinook.ClientModels.Album>();
            CreateMap<Chinook.Models.Artist, Chinook.ClientModels.Artist>();
            CreateMap<Chinook.Models.Playlist, Chinook.ClientModels.Playlist>();
        }
    }
}
