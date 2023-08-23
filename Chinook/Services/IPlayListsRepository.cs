using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Services
{
    public interface IPlayListsRepository
    {
        Task<List<Models.Playlist>> GetPlayLists(string userId);
        Task<ClientModels.Playlist> GetPlayList(string userId , long playListId);
        Task<List<Models.Playlist>> CreatePlaylists(PlaylistTrack playlistTrack, string playListName, string userId);
    }
}
