using AutoMapper;
using Azure;
using Azure.Core;
using Chinook.ClientModels;
using Chinook.Helper;
using Chinook.Models;
using Chinook.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Composition;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Chinook.Services
{
    public class PlayListsRepository : IPlayListsRepository
    {
        private readonly IPlayListsProvider _playListsProvider;
        private readonly IMapper _mapper;
        private string userId;
        public PlayListsRepository(IPlayListsProvider playListsProvider, IMapper mapper, IAuthentication authentication)
        {
            _playListsProvider = playListsProvider;
            _mapper = mapper;
            userId = authentication.CurrentUserId;
        }

        //Creating playlist per user
        public bool CreatePlaylists(long trackId, string playListName)
        {
             return _playListsProvider.CreatePlaylists(trackId, playListName, userId);
        }

        //Get action
        //Return a single "PlayList" data 
        public async Task<PlaylistViewModel> GetPlayList(long playListId)
        {
                var playlist = await _playListsProvider.GetPlayList(playListId);

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

        //Get action
        //Returns List of "PlayList" data 
        public async Task<List<PlaylistsViewModel>> GetPlayLists()
        {
            var playlists = await _playListsProvider.GetPlayLists(userId);
            return _mapper.Map<List<PlaylistsViewModel>>(playlists);
        }
    }
}
