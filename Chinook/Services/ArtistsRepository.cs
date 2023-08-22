using Chinook.ClientModels;
using Chinook.Migrations;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Chinook.Services
{
    public class ArtistsRepository : IArtistsRepository
    {
        private readonly IDbContextFactory<ChinookContext> _dbContextFactory;
        private readonly ArtistData _artistData;
        private ChinookContext dbContext;
        public ArtistsRepository(IDbContextFactory<ChinookContext> dbContextFactory, ArtistData artistData)
        {
            _dbContextFactory = dbContextFactory;
            _artistData = artistData;
        }

        //Get action
        //Returns "ArtistData" 
        public async Task<ArtistData> GetArtists(long artistId, string userId)
        {
            dbContext = await _dbContextFactory.CreateDbContextAsync();
            _artistData.Artist = dbContext.Artists.SingleOrDefault(a => a.ArtistId == artistId);
            _artistData.Tracks = dbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
                .Include(a => a.Album)
                .Select(t => new PlaylistTrack()
                {
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == "My Favorites")).Any()
                })
                .ToList();

            return _artistData;
        }

        //Add the track to the favorite list
        public async Task AddFavorite(PlaylistTrack playlistTrack, string userId)
        {
            dbContext = await _dbContextFactory.CreateDbContextAsync();
            var track = dbContext.Tracks.FirstOrDefault(t => t.TrackId == playlistTrack.TrackId);

            Random rnd = new Random();
            int index = rnd.Next();

            if (track != null)
            {
                //Create a playlist for favorite if it is not exist
                if (!CheckUserPlayListExistence(dbContext, userId))
                {
                    var playList = new Chinook.Models.Playlist() { PlaylistId = index, Name = Constants.FavoriteList.Name };
                    playList.Tracks.Add(track);
                    dbContext.Playlists.Add(playList);

                    var userPlayList = new Chinook.Models.UserPlaylist() { PlaylistId = index, UserId = userId };
                    dbContext.UserPlaylists.Add(userPlayList);
                }
                else
                {
                    var lists = dbContext.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId));
                    var favList = lists.First(p => p.Name == Constants.FavoriteList.Name);
                    favList.Tracks.Add(track);
                    dbContext.Playlists.Update(favList);
                }
                try
                {
                    await dbContext.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task UnFavorite(PlaylistTrack playlistTrack, string currentUserId)
        {
            dbContext = await _dbContextFactory.CreateDbContextAsync();
            var track = dbContext.Tracks.FirstOrDefault(t => t.TrackId == playlistTrack.TrackId);

            if (track != null)
            {
                //Remove a track from favorite list
                try
                {
                    var userPlayLists = dbContext.UserPlaylists.Where(up => up.UserId == currentUserId).ToList();
                    var favPlayList = dbContext.Playlists.Include(p => p.Tracks).First(p => p.Name == Constants.FavoriteList.Name);
                    favPlayList.Tracks.Remove(track);
                    dbContext.Playlists.Update(favPlayList);
                    await dbContext.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private bool CheckUserPlayListExistence(ChinookContext dbContext, string userId)
        {
            var userPlayLists = dbContext.UserPlaylists.Where(up => up.UserId == userId).ToList();
            var favPlayLists = dbContext.Playlists.Where(p => p.Name == Constants.FavoriteList.Name);
            return favPlayLists.Any(p => p.UserPlaylists.Any(up => up.UserId == userId));
        }
    }
}
