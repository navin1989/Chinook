using Azure.Core;
using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Chinook.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AlbumsController : ControllerBase
    {
        private readonly IDbContextFactory<ChinookContext> _dbContextFactory;
        private readonly ArtistData _artistData;
        public AlbumsController(IDbContextFactory<ChinookContext> dbContextFactory, ArtistData artistData)
        {
            _dbContextFactory = dbContextFactory;
            _artistData = artistData;
        }

        [HttpGet]
        public async Task<ActionResult> GetAlbums()
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                return Ok(dbContext.Artists.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{artistId:long}/{userId}")]
        public async Task<ActionResult> GetArtistsData(long artistId, string userId)
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            _artistData.Artist = dbContext.Artists.SingleOrDefault(a => a.ArtistId == artistId);

            _artistData.Tracks = dbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
                .Include(a => a.Album)
                .Select(t => new PlaylistTrack()
                {
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == "Favorites")).Any()
                })
                .ToList();

            return Ok(_artistData);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetPlayLists(string userId)
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                var up = dbContext.UserPlaylists;
                var filteredList = dbContext.Playlists.Where(p => up.Any(a => a.PlaylistId == p.PlaylistId)); //Filters only the current user's playlists
                if (filteredList.Any())
                {
                    return Ok(filteredList.ToList());
                }
                else return Ok(new List<Models.Playlist>());

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("{playListName}/{userId}")]
        public async Task<ActionResult> CreatePlayList(string playListName , string userId)
        {
            var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var data = await reader.ReadToEndAsync().ConfigureAwait(false);

            var playlistTrack = JsonSerializer.Deserialize<PlaylistTrack>(data);
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var track = dbContext.Tracks.FirstOrDefault(t => t.TrackId == playlistTrack.TrackId);
            Random rnd = new Random();
            int index = rnd.Next();
            try
            {
                if (dbContext.Playlists.FirstOrDefault(p => p.Name == playListName) != null)
                {
                    var playList = dbContext.Playlists.FirstOrDefault(p => p.Name == playListName);
                    playList.Tracks.Add(track);
                    dbContext.Playlists.Update(playList);
                }
                else
                {
                    var playList = new Chinook.Models.Playlist() { PlaylistId = index, Name = playListName };
                    playList.Tracks.Add(track);
                    dbContext.Playlists.Add(playList);

                    var userPlayList = new Chinook.Models.UserPlaylist() { PlaylistId = index, UserId = userId };
                    dbContext.UserPlaylists.Add(userPlayList);
                }

                await dbContext.SaveChangesAsync();
                return GetPlayLists(userId).Result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpPost("{userId}")]
        public async Task AddToFavorite(string userId)
        {
            var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var data = await reader.ReadToEndAsync().ConfigureAwait(false);

            var playlistTrack = JsonSerializer.Deserialize<PlaylistTrack>(data);
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var track = dbContext.Tracks.FirstOrDefault(t => t.TrackId == playlistTrack.TrackId);

            if (track != null)
            {
                //Create a playlist for favorite if it is not exist
                //One time creation
                if (dbContext.Playlists.FirstOrDefault(p => p.PlaylistId == Constants.FavoriteList.PlaylistId) == null)
                {
                    var playList = new Chinook.Models.Playlist() { PlaylistId = Constants.FavoriteList.PlaylistId, Name = Constants.FavoriteList.Name };
                    playList.Tracks.Add(track);
                    dbContext.Playlists.Add(playList);
                }
                else
                {
                    var playList = dbContext.Playlists.FirstOrDefault(p => p.PlaylistId == Constants.FavoriteList.PlaylistId);
                    playList.Tracks.Add(track);
                    dbContext.Playlists.Update(playList);
                }
                //Create a UserPlaylists for favorite if it is not exist
                //One time creation
                if (dbContext.UserPlaylists.FirstOrDefault(p => p.PlaylistId == Constants.FavoriteList.PlaylistId) == null)
                {
                    var userPlayList = new Chinook.Models.UserPlaylist() { PlaylistId = Constants.FavoriteList.PlaylistId, UserId = userId };
                    dbContext.UserPlaylists.Add(userPlayList);
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
    }
}
