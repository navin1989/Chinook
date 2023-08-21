using Chinook.ClientModels;

namespace Chinook.Models
{
    public interface IArtistsRepository 
    {
        Task<ArtistData> GetArtists(long artistId , string currentUserId);
        Task AddFavorite(PlaylistTrack playlistTrack, string currentUserId);
    }
}
