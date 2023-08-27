using Azure;
using Azure.Core;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Chinook.Provider
{
    public class PlayListsRepository : IPlayListsRepository
    {
        private readonly ChinookContext _dbContext;
        public PlayListsRepository(ChinookContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Creating playlist per user
        public bool CreatePlaylists(long trackId, string playListName, string userId)
        {
            var track = _dbContext.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            int index = _dbContext.Playlists.Count() + 1;
            bool newListCreated = false;
            try
            {
                if (CheckForPlaylistExistence(_dbContext, userId, playListName))
                {
                    var playlists = _dbContext.Playlists.Include(p => p.Tracks).FirstOrDefault(p => p.Name == playListName && p.UserPlaylists.Any(up => up.UserId == userId));

                    playlists.Tracks.Add(track);
                    track.Playlists.Add(playlists);
                }
                else
                {
                    var playList = new Playlist() { PlaylistId = index , Name = playListName };
                    var userPlayList = new  UserPlaylist() { Playlist = playList, UserId = userId };

                    playList.Tracks.Add(track);
                    _dbContext.UserPlaylists.Add(userPlayList);
                    _dbContext.Playlists.Add(playList);
                    newListCreated = true;
                }

                if (_dbContext.SaveChanges() > 0 && newListCreated) return true;
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Get action
        //Return a single "ClientPlayList" data 
        public async Task<Playlist?> GetPlayList(long playListId)
        {
            try
            {
                return _dbContext.Playlists
               .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
               .Include(p => p.Tracks).ThenInclude(c => c.Playlists).ThenInclude(c => c.UserPlaylists)
               .Where(p => p.PlaylistId == playListId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Get action
        //Returns List of "PlayList" data 
        async Task<List<Playlist>> IPlayListsRepository.GetPlayLists(string userId)
        {
            try
            {
                var up = _dbContext.UserPlaylists.Where(up => up.UserId == userId);
                return await _dbContext.Playlists.Where(p => up.Any(a => a.PlaylistId == p.PlaylistId)).OrderBy(c => c.Name == Constants.FavoriteList.Name ? 1 : c.PlaylistId).ToListAsync(); //Filters only the current user's playlists
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool CheckForPlaylistExistence(ChinookContext dbContext, string userId, string playListName)
        {
            var playlists = dbContext.Playlists.Include(p => p.UserPlaylists).Where(p => p.UserPlaylists.Any(up => up.UserId == userId));
            bool playListExist = playlists.Any(p => p.Name == playListName);
            return playListExist;
        }
    }
}
