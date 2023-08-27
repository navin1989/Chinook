using AutoMapper;
using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;
using Chinook.Provider;
using Chinook.Models;

namespace Chinook.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        public AlbumService(IAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository;
        }
        public async Task<List<ArtistsViewModel>> GetAlbums()
        {
            var albums = await _albumRepository.GetAlbums();
            return albums.Select(a => new ArtistsViewModel
            {
                Name = a.Name,
                AlbumsCount = a.Albums.Count,
                ArtistId = a.ArtistId,

            }).ToList();
        }
    }
}
