using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Provider;

namespace Chinook.Mapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Album, AlbumViewModel>();
            CreateMap<Artist, ArtistsViewModel>()
                .ForMember(c => c.AlbumsCount, x => x.MapFrom(v => v.Albums.Count()));
            CreateMap<Playlist, PlaylistsViewModel>();
            CreateMap<ArtistData, ArtistDataViewModel>();
        }
    }
}
