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
        IArtistsService ArtistsRepository { get; set; }

        [Inject]
        IPlayListsService PlayListsRepo { get; set; }

        [Inject]
        InvokeSideBarService InvokeNavBar { get; set; }


        private ArtistViewModel Artist;
        private List<PlaylistTrackViewModel> Tracks;
        private List<PlaylistsViewModel> PlayLists = new();
        private PlaylistTrackViewModel SelectedTrack;
        private string InfoMessage;
        private string PlayListName { get; set; }
        private string ExistingPlayListName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await InvokeAsync(StateHasChanged);
                var artistModel = await ArtistsRepository.GetArtists(ArtistId);
                Artist = artistModel.Artist;
                Tracks = artistModel.Tracks;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void FavoriteTrack(long trackId)
        {
            try
            {
                var track = Tracks.First(t => t.TrackId == trackId);
                ArtistsRepository.AddFavorite(trackId);
                InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";
                InvokeNavBar.Invoke();
                _ = OnInitializedAsync();
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
            InvokeNavBar.Invoke();
            _ = OnInitializedAsync();
        }

        private async Task OpenPlaylistDialog(long trackId)
        {
            PlayListName = "";
            PlayLists = await PlayListsRepo.GetPlayLists();
            CloseInfoMessage();
            SelectedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            PlaylistDialog.Open();
        }

        private void AddTrackToPlaylist()
        {
            if (string.IsNullOrEmpty(PlayListName) && !string.IsNullOrEmpty(ExistingPlayListName))
            {
                PlayListName = ExistingPlayListName;
            }
            if(!string.IsNullOrEmpty(PlayListName))
            {
                bool newListCreated = PlayListsRepo.CreatePlaylists(SelectedTrack.TrackId, PlayListName);
                CloseInfoMessage();
                InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist - {@PlayListName}.";
                PlaylistDialog.Close();
                if(newListCreated)
                {
                    InvokeNavBar.Invoke();
                }
            }          

        }

        private void CloseInfoMessage()
        {
            InfoMessage = "";
        }

        private void SelectedPlayListChanged(ChangeEventArgs e)
        {
            if (e.Value is not null)
            {
                ExistingPlayListName = (string)e.Value;
            }
        }
    }
}