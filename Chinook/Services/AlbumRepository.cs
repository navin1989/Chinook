using AutoMapper;
using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;
using Chinook.Provider;

namespace Chinook.Services
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly IMapper _mapper;
        private readonly IAlbumProvider _albumProvider;
        public AlbumRepository(IMapper mapper, IAlbumProvider albumProvider)
        {
            _mapper = mapper;
            _albumProvider = albumProvider;
        }
        public async Task<List<ArtistsViewModel>> GetAlbums()
        {
                var albums = await _albumProvider.GetAlbums();
                return _mapper.Map<List<ArtistsViewModel>>(albums);
        }
    }
}
