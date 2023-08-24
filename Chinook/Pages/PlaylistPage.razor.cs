using global::Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Chinook.ClientModels;
using Chinook.Services;
using System.Security.Claims;

namespace Chinook.Pages
{
    public partial class PlaylistPage
    {
        [Parameter]
        public long PlaylistId { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationState { get; set; }

        [Inject]
        IPlayListsRepository PlayListRepository { get; set; }

        [Inject]
        IArtistsRepository ArtistsRepository { get; set; }

        private PlaylistViewModel Playlist;
        private string CurrentUserId;
        private string InfoMessage;
        //Used this overridden method, to make sure the aside NavLinks are rereshing the page.
        protected override async Task OnParametersSetAsync()
        {
            await HelperMethod();
        }

        protected override async Task OnInitializedAsync()
        {
            await HelperMethod();
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

        private void FavoriteTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";
            ArtistsRepository.AddFavorite(trackId);
        }

        private async Task UnfavoriteTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            await ArtistsRepository.RemoveFavoriteTrack(trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
        }

        private void RemoveTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            ArtistsRepository.RemoveTrack(trackId, PlaylistId);
            CloseInfoMessage();
        }

        private void CloseInfoMessage()
        {
            InfoMessage = "";
        }

        private async Task HelperMethod()
        {
            await InvokeAsync(StateHasChanged);
            Playlist = await PlayListRepository.GetPlayList(PlaylistId);
        }
    }
}