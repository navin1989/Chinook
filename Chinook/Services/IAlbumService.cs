using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IAlbumService
    {
        Task<List<ArtistsViewModel>> GetAlbums();
    }
}
