using Chinook.ClientModels;

namespace Chinook.Models
{
    public interface IPlayListsRepository
    {
        Task<List<Playlist>> GetPlayLists(string userId);
        Task<List<Playlist>> CreatePlaylists(PlaylistTrack playlistTrack, string playListName,  string userId);
    }
}
