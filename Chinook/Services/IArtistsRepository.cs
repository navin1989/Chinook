using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IArtistsRepository
    {
        Task<ArtistData> GetArtists(long artistId, string currentUserId);
        Task AddFavorite(PlaylistTrack playlistTrack, string currentUserId);
        Task RemoveFavoriteTrack(PlaylistTrack playlistTrack);
        Task RemoveTrack(PlaylistTrack playlistTrack, long playListId);
    }
}
