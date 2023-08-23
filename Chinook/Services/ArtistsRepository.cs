using AutoMapper;
using Chinook.ClientModels;
using Chinook.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
        private readonly ChinookContext _dbContext;
        private readonly ArtistData _artistData;
        private IMapper _mapper;
        private ChinookContext dbContext;
        public ArtistsRepository(ChinookContext dbContext, ArtistData artistData , IMapper mapper)
        {
            _dbContext = dbContext;
            _artistData = artistData;
            _mapper = mapper;
        }

        //Get action
        //Returns "ArtistData" 
        public async Task<ArtistData> GetArtists(long artistId, string userId)
        {
            var list = _dbContext.Artists.SingleOrDefault(a => a.ArtistId == artistId);
            _artistData.Artist = _mapper.Map<Artist>(list);

            _artistData.Tracks = _dbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
                .Include(a => a.Album)
                .Select(t => new PlaylistTrack()
                {
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == Constants.FavoriteList.Name)).Any()
                })
                .ToList();

            return _artistData;
        }

        //Add the track to the favorite list
        public async Task AddFavorite(PlaylistTrack playlistTrack, string userId)
        {
            var track = _dbContext.Tracks.FirstOrDefault(t => t.TrackId == playlistTrack.TrackId);

            Random rnd = new Random();
            int index = rnd.Next();

            if (track != null)
            {
                //Create a playlist for favorite if it is not exist
                if (!CheckUserPlayListExistence(_dbContext, userId))
                {
                    var playList = new Chinook.Models.Playlist() { PlaylistId = index, Name = Constants.FavoriteList.Name };
                    playList.Tracks.Add(track);
                    _dbContext.Playlists.Add(playList);

                    var userPlayList = new Chinook.Models.UserPlaylist() { PlaylistId = index, UserId = userId };
                    _dbContext.UserPlaylists.Add(userPlayList);
                }
                else
                {
                    var lists = _dbContext.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId));
                    var favList = lists.First(p => p.Name == Constants.FavoriteList.Name);
                    favList.Tracks.Add(track);
                    _dbContext.Playlists.Update(favList);
                }
                try
                {
                    await _dbContext.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        //Delete from a play list
        public async Task RemoveTrack(PlaylistTrack playlistTrack, long PlaylistId)
        {
            var track = _dbContext.Tracks.FirstOrDefault(t => t.TrackId == playlistTrack.TrackId);
            if (track != null)
            {
                try
                {
                    var favPlayList = _dbContext.Playlists.Include(p => p.Tracks).First(p => p.PlaylistId == PlaylistId);
                    track.Playlists.Remove(favPlayList);
                    favPlayList.Tracks.Remove(track);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

       // Remove from favorite list
        public async Task RemoveFavoriteTrack(PlaylistTrack playlistTrack)
        {
            var track = _dbContext.Tracks.FirstOrDefault(t => t.TrackId == playlistTrack.TrackId);

            if (track != null)
            {
                try
                {
                    var favPlayList = _dbContext.Playlists.Include(p => p.Tracks).First(p => p.Name == Constants.FavoriteList.Name);
                    track.Playlists.Remove(favPlayList);
                    favPlayList.Tracks.Remove(track);
                    await _dbContext.SaveChangesAsync();

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
