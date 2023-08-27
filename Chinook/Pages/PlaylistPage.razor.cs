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
        IPlayListsService PlayListRepository { get; set; }

        [Inject]
        IArtistsService ArtistsRepository { get; set; }

        [Inject]
        InvokeSideBarService InvokeNavBar { get; set; }

        private PlaylistViewModel Playlist;
        private string InfoMessage;

        protected override async Task OnInitializedAsync()
        {
            await HelperMethod();
        }
        protected override async Task OnParametersSetAsync()
        {
            await HelperMethod();
        }

        private void FavoriteTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";
            ArtistsRepository.AddFavorite(trackId);
            InvokeNavBar.Invoke();
        }

        private void UnfavoriteTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            ArtistsRepository.RemoveFavoriteTrack(trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
            InvokeNavBar.Invoke();
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