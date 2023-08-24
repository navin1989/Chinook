using Chinook.ClientModels;
using Chinook.Provider;

namespace Chinook.Services
{
    public interface IArtistsRepository
    {
        Task<ArtistDataViewModel> GetArtists(long artistId);
        Task AddFavorite(long trackId);
        Task RemoveFavoriteTrack(long trackId);
        Task RemoveTrack(long trackId, long playListId);
    }
}
