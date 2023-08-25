using global::Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Chinook.ClientModels;
using Chinook.Services;
using System.Security.Claims;

namespace Chinook.Shared
{
    public partial class NavMenu
    {
        //Injecting "IPlayLists"
        [Inject]
        IPlayListsRepository PlayListsRepo { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationState { get; set; }

        private bool CollapseNavMenu = true;
        private string CurrentUserId = "";
        private List<PlaylistsViewModel> Playlists;
        private PlaylistsViewModel FavPlaylist;
        private string? NavMenuCssClass => CollapseNavMenu ? "collapse" : null;

        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            await LoadData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await OnInitializedAsync();
        }

        private async Task LoadData()
        {
            CurrentUserId = await GetUserId();
            Playlists = await PlayListsRepo.GetPlayLists();
            FavPlaylist = Playlists.FirstOrDefault(p => p.Name == Constants.FavoriteList.Name);
        }

        private void ToggleNavMenu()
        {
            CollapseNavMenu = !CollapseNavMenu;
        }

        private async Task<string> GetUserId()
        {
            var user = (await AuthenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }
    }
}