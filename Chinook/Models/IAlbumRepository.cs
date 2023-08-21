namespace Chinook.Models
{
    public interface IAlbumRepository
    {
        Task<List<Artist>> GetAlbums();
    }
}
