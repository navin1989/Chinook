using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Services
{
    public interface IArtistsRepository
    {
        Task<ArtistData> GetArtists(long artistId, string currentUserId);
        Task AddFavorite(PlaylistTrack playlistTrack, string currentUserId);
        Task UnFavorite(PlaylistTrack playlistTrack, string currentUserId);
    }
}
