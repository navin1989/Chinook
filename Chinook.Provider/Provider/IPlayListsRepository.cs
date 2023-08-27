using Chinook.Models;

namespace Chinook.Provider
{
    public interface IPlayListsRepository
    {
        Task<List<Playlist>> GetPlayLists(string userId);
        Task<Playlist> GetPlayList(long playListId);
        bool CreatePlaylists(long trackId, string playListName, string userId);
    }
}
