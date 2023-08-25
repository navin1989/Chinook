using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Provider;

namespace Chinook.Services
{
    public interface IPlayListsRepository
    {
        Task<List<PlaylistsViewModel>> GetPlayLists();
        Task<PlaylistViewModel> GetPlayList(long playListId);
        bool CreatePlaylists(long trackId, string playListName);
    }
}
