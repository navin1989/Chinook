using Chinook.Models;

namespace Chinook.Provider
{
    public interface IAlbumRepository
    {
        Task<List<Artist>> GetAlbums();
    }
}
