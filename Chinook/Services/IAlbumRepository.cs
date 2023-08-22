using Chinook.Models;

namespace Chinook.Services
{
    public interface IAlbumRepository
    {
        Task<List<Artist>> GetAlbums();
    }
}
