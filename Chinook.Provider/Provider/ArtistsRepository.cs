using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Provider
{
    public class ArtistsRepository : IArtistsRepository
    {
        private readonly ChinookContext _dbContext;
        private ArtistData _artistData;
        public ArtistsRepository(ChinookContext dbContext)
        {
            _dbContext = dbContext;
            _artistData = new ArtistData();
        }

        //Get action
        //Returns "ArtistData" 
        public async Task<ArtistData> GetArtists(long artistId, string userId)
        {
            _artistData.Artist = _dbContext.Artists.Include(c => c.Albums).SingleOrDefault(a => a.ArtistId == artistId);

            _artistData.Tracks = await _dbContext.Tracks.Include(c => c.Album).ThenInclude(c => c.Artist)
                .Include(c => c.Playlists).ThenInclude(c => c.UserPlaylists).Where(a => a.Album.ArtistId == artistId)
                .ToListAsync();

            return _artistData;
        }

        //Add the track to the favorite list
        public async Task AddFavorite(long trackId, string userId)
        {
            var track = _dbContext.Tracks.FirstOrDefault(t => t.TrackId == trackId);

            Random rnd = new Random();
            int index = rnd.Next();

            if (track != null)
            {

                //Create a playlist for favorite if it is not exist
                if (!CheckUserPlayListExistence(_dbContext, userId))
                {
                    var playList = new Playlist() { PlaylistId = index, Name = Constants.FavoriteList.Name };
                    playList.Tracks.Add(track);
                    _dbContext.Playlists.Add(playList);

                    var userPlayList = new UserPlaylist() { Playlist = playList, UserId = userId };
                    _dbContext.UserPlaylists.Add(userPlayList);
                }
                else
                {
                    var lists = _dbContext.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId));
                    var favList = lists.First(p => p.Name == Constants.FavoriteList.Name);
                    favList.Tracks.Add(track);
                    track.Playlists.Add(favList);
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
        public async Task RemoveTrack(long TrackId, long PlaylistId)
        {
            var track = _dbContext.Tracks.FirstOrDefault(t => t.TrackId == TrackId);
            if (track != null)
            {
                try
                {
                    var playList = _dbContext.Playlists.Include(p => p.Tracks).First(p => p.PlaylistId == PlaylistId);
                    track.Playlists.Remove(playList);
                    playList.Tracks.Remove(track);

                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        // Remove from favorite list
        public async Task RemoveFavoriteTrack(long TrackId, string userId)
        {
            try
            {
                var track = _dbContext.Tracks.Include(t => t.Playlists).FirstOrDefault(t => t.TrackId == TrackId);
                if (track != null)
                {
                    var favPlayList = _dbContext.Playlists.Include(p => p.Tracks)
                                      .FirstOrDefault(p => p.Name == Constants.FavoriteList.Name && p.UserPlaylists.Any(up => up.UserId == userId));
                    track.Playlists.Remove(favPlayList);
                    favPlayList.Tracks.Remove(track);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
