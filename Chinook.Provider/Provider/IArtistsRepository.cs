using Chinook.Models;

namespace Chinook.Provider
{
    public interface IArtistsRepository
    {
        Task<ArtistData> GetArtistAndTracks(long artistId, string currentUserId);
        Task AddFavorite(long trackId, string currentUserId);
        Task RemoveFavoriteTrack(long trackId, string userId);
        Task RemoveTrack(long trackId, long playListId);
    }
}
