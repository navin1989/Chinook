using Chinook.ClientModels;
using Chinook.Provider;

namespace Chinook.Services
{
    public interface IArtistsService
    {
        Task<ArtistDataViewModel> GetArtistAndTracks(long artistId);
        Task AddFavorite(long trackId);
        Task RemoveFavoriteTrack(long trackId);
        Task RemoveTrack(long trackId, long playListId);
    }
}
