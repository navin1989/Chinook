using Chinook.Models;

namespace Chinook.Provider
{
    public interface IAlbumProvider
    {
        Task<List<Artist>> GetAlbums();
    }
}
