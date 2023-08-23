using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IAlbumRepository
    {
        Task<List<Artist>> GetAlbums();
    }
}
