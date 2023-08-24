using AutoMapper;
using Chinook.ClientModels;
using Chinook.Helper;
using Chinook.Provider;

namespace Chinook.Services
{
    public class ArtistsRepository : IArtistsRepository
    {
        private IMapper _mapper;
        private IArtistsProvider _artistsProvider;
        private string userId;
        public ArtistsRepository(IMapper mapper , IArtistsProvider artistsProvider, IAuthentication authentication)
        {
            _mapper = mapper;
            _artistsProvider = artistsProvider;
            userId = authentication.CurrentUserId;
        }

        //Get action
        //Returns "ArtistData" 
        public async Task<ArtistDataViewModel> GetArtists(long artistId)
        {
            try
            {
                 var data = await _artistsProvider.GetArtists(artistId, userId);

                return new ArtistDataViewModel
                {
                    Tracks = data.Tracks.Select(t => new PlaylistTrackViewModel
                    {
                        AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                        TrackId = t.TrackId,
                        TrackName = t.Name,
                        IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == Constants.FavoriteList.Name)).Any()
                    }).ToList(),
                    Artist = new ArtistViewModel { Name = data.Artist.Name }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Add the track to the favorite list
        public async Task AddFavorite(long trackId)
        {
            try
            {
               await _artistsProvider.AddFavorite(trackId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            
        }

        //Delete from a play list
        public async Task RemoveTrack(long trackId, long PlaylistId)
        {
            try
            {
                await _artistsProvider.RemoveTrack(trackId, PlaylistId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

       // Remove from favorite list
        public async Task RemoveFavoriteTrack(long trackId)
        {
            try
            {
                await _artistsProvider.RemoveFavoriteTrack(trackId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
