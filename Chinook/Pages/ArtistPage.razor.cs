using global::Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Chinook.Shared.Components;
using Chinook.Services;
using System.Security.Claims;
using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Pages
{
    public partial class ArtistPage
    {
        [Parameter]
        public long ArtistId { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationState { get; set; }
        private Modal PlaylistDialog { get; set; }

        [Inject]
        IArtistsRepository ArtistsRepository { get; set; }

        [Inject]
        IPlayListsRepository PlayListsRepo { get; set; }

        private ArtistViewModel Artist;
        private List<PlaylistTrackViewModel> Tracks;
        private List<PlaylistsViewModel> PlayLists;
        private DbContext DbContext;
        private PlaylistTrackViewModel SelectedTrack;
        private string InfoMessage;
        private string CurrentUserId;
        private string PlayListName { get; set; }
        private string PlayListId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await InvokeAsync(StateHasChanged);
                CurrentUserId = await GetUserId();
                var artistModel = await ArtistsRepository.GetArtists(ArtistId);
                Artist = artistModel.Artist;
                Tracks = artistModel.Tracks;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await OnInitializedAsync();
        }

        private async Task<string> GetUserId()
        {
            var user = (await AuthenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }

        private void FavoriteTrack(long trackId, string userId)
        {
            try
            {
                var track = Tracks.First(t => t.TrackId == trackId);
                ArtistsRepository.AddFavorite(trackId);
                InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task UnfavoriteTrack(long trackId)
        {
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            await ArtistsRepository.RemoveFavoriteTrack(trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
        }

        private async Task OpenPlaylistDialog(long trackId)
        {
            PlayLists = await PlayListsRepo.GetPlayLists();
            CloseInfoMessage();
            SelectedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            PlaylistDialog.Open();
        }

        private void AddTrackToPlaylist()
        {
            CloseInfoMessage();
            InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {{playlist name}}.";
            var payLists = PlayListsRepo.CreatePlaylists(SelectedTrack, @PlayListName);
            PlaylistDialog.Close();
        }

        private void CloseInfoMessage()
        {
            InfoMessage = "";
        }

        private void SelectedPlayListChanged(ChangeEventArgs e)
        {
            if (e.Value is not null)
            {
                PlayListName = (string)e.Value;
            }
        }
    }
}