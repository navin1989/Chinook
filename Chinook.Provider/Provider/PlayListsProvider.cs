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
    public class PlayListsProvider : IPlayListsProvider
    {
        private readonly ChinookContext _dbContext;
        public PlayListsProvider(ChinookContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Creating playlist per user
        public bool CreatePlaylists(long trackId, string playListName, string userId)
        {
            var track = _dbContext.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            Random rnd = new Random();
            int index = rnd.Next();
            try
            {
                if (CheckForPlaylistExistence(_dbContext, userId , playListName))
                {
                    var playlists = _dbContext.Playlists.Include(p => p.UserPlaylists).Where(p => p.UserPlaylists.Any(up => up.UserId == userId));
                    var list = playlists.First(p => p.Name == playListName);
                    list.Tracks.Add(track);
                    _dbContext.Playlists.Update(list);
                }
                else
                {
                    var playList = new Chinook.Models.Playlist() { PlaylistId = index, Name = playListName };
                    playList.Tracks.Add(track);
                    _dbContext.Playlists.Add(playList);

                    var userPlayList = new Chinook.Models.UserPlaylist() { PlaylistId = index, UserId = userId };
                    _dbContext.UserPlaylists.Add(userPlayList);
                }

                _dbContext.SaveChanges();
                return true;
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
            return _dbContext.Playlists
                        .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
                        .Include(p=>p.Tracks).ThenInclude(c => c.Playlists).ThenInclude(c => c.UserPlaylists)
                        .Where(p => p.PlaylistId == playListId).FirstOrDefault();
                     
        }

        //Get action
        //Returns List of "PlayList" data 
        async Task<List<Playlist>> IPlayListsProvider.GetPlayLists(string userId)
        {
            try
            {
                var up = _dbContext.UserPlaylists.Where(up => up.UserId == userId);
                return await _dbContext.Playlists.Where(p => up.Any(a => a.PlaylistId == p.PlaylistId)).ToListAsync(); //Filters only the current user's playlists
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
