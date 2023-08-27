using AutoMapper;
using Chinook.ClientModels;
using Chinook.Helper;
using Chinook.Provider;

namespace Chinook.Services
{
    public class ArtistsService : IArtistsService
    {
        private IArtistsRepository _artistsRepository;
        private string userId;
        public ArtistsService(IArtistsRepository artistsRepository, IAuthentication authentication)
        {
            _artistsRepository = artistsRepository;
            userId = authentication.CurrentUserId;
        }

        //Get action
        //Returns "ArtistData" 
        public async Task<ArtistDataViewModel> GetArtists(long artistId)
        {
                var data = await _artistsRepository.GetArtists(artistId, userId);

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

        //Add the track to the favorite list
        public async Task AddFavorite(long trackId)
        {
            await _artistsRepository.AddFavorite(trackId, userId);          
        }

        // Remove from favorite list
        public async Task RemoveFavoriteTrack(long trackId)
        {
            await _artistsRepository.RemoveFavoriteTrack(trackId, userId);
        }

        //Delete from a play list
        public async Task RemoveTrack(long trackId, long PlaylistId)
        {
            await _artistsRepository.RemoveTrack(trackId, PlaylistId);
        }

    }
}
