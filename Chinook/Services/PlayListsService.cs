using AutoMapper;
using Chinook.ClientModels;
using Chinook.Helper;
using Chinook.Provider;

namespace Chinook.Services
{
    public class PlayListsService : IPlayListsService
    {
        private readonly IPlayListsRepository _playListsRepository;
        private string userId;
        public PlayListsService(IPlayListsRepository playListsRepository, IAuthentication authentication)
        {
            _playListsRepository = playListsRepository;
            userId = authentication.CurrentUserId;
        }

        //Creating playlist per user
        public bool CreatePlaylists(long trackId, string playListName)
        {
            return _playListsRepository.CreatePlaylists(trackId, playListName, userId);
        }

        //Returns a single "PlayList" data 
        public async Task<PlaylistViewModel> GetPlayList(long playListId)
        {
            var playlist = await _playListsRepository.GetPlayList(playListId);

            return new PlaylistViewModel
            {
                Name = playlist.Name,
                Tracks = playlist.Tracks.Select(t => new PlaylistTrackViewModel
                {
                    AlbumTitle = t.Album.Title,
                    ArtistName = t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == Constants.FavoriteList.Name)).Any()
                }).ToList()
            };
        }

        //Returns List of "PlayList" data 
        public async Task<List<PlaylistsViewModel>> GetPlayLists()
        {
            var playlists = await _playListsRepository.GetPlayLists(userId);
            return playlists.Select(p => new PlaylistsViewModel
            {
                Name = p.Name,
                PlaylistId = p.PlaylistId
            }).ToList();
        }
    }
}
